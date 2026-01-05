using MindWeaveClient.AuthenticationService;
using MindWeaveClient.Services.Abstractions;
using System;
using System.Net.Sockets;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    /// <summary>
    /// Authentication service implementation that handles login, registration, and logout.
    /// Integrates heartbeat service initialization after successful login.
    /// 
    /// CRITICAL: After successful login, this service:
    /// 1. Sets the session
    /// 2. Connects the social service
    /// 3. Starts the heartbeat service ← NEW
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ISocialService socialService;
        private readonly IHeartbeatService heartbeatService;

        /// <summary>
        /// Creates a new AuthenticationService with all required dependencies.
        /// </summary>
        public AuthenticationService(ISocialService socialService, IHeartbeatService heartbeatService)
        {
            this.socialService = socialService;
            this.heartbeatService = heartbeatService;
        }

        /// <summary>
        /// Authenticates a user and initializes all post-login services including heartbeat.
        /// </summary>
        public async Task<LoginServiceResultDto> loginAsync(string email, string password)
        {
            var loginCredentials = new LoginDto
            {
                Email = email,
                Password = password
            };

            LoginResultDto result = await executeServiceCallAsync(async (client) =>
                await client.loginAsync(loginCredentials));

            if (result.OperationResult.Success)
            {
                // Step 1: Set the session
                SessionService.setSession(result.PlayerId, result.Username, result.AvatarPath);

                // Step 2: Connect social service
                bool socialConnected = await connectSocialServiceSafeAsync(result.Username);

                // ╔═══════════════════════════════════════════════════════════════════╗
                // ║ Step 3: CRITICAL - Start heartbeat service after successful login ║
                // ╚═══════════════════════════════════════════════════════════════════╝
                bool heartbeatStarted = await startHeartbeatServiceSafeAsync(result.Username);

                if (!heartbeatStarted)
                {
                    System.Diagnostics.Debug.WriteLine("[AUTH] Warning: Heartbeat service failed to start");
                    // Continue anyway - the user can still use the app, 
                    // but may be disconnected unexpectedly by the server
                }

                return new LoginServiceResultDto(result, socialConnected, true);
            }
            else
            {
                return new LoginServiceResultDto(result);
            }
        }

        /// <summary>
        /// Resends the verification code to the specified email.
        /// </summary>
        public async Task<OperationResultDto> resendVerificationCodeAsync(string email)
        {
            return await executeServiceCallAsync(async (client) =>
                await client.resendVerificationCodeAsync(email));
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        public async Task<OperationResultDto> registerAsync(UserProfileDto profile, string password)
        {
            return await executeServiceCallAsync(async (client) =>
                await client.registerAsync(profile, password));
        }

        /// <summary>
        /// Verifies a user account with the provided code.
        /// </summary>
        public async Task<OperationResultDto> verifyAccountAsync(string email, string code)
        {
            return await executeServiceCallAsync(async (client) =>
                await client.verifyAccountAsync(email, code));
        }

        /// <summary>
        /// Sends a password recovery code to the specified email.
        /// </summary>
        public async Task<OperationResultDto> sendPasswordRecoveryCodeAsync(string email)
        {
            return await executeServiceCallAsync(async (client) =>
                await client.sendPasswordRecoveryCodeAsync(email));
        }

        /// <summary>
        /// Resets the password using the provided recovery code.
        /// </summary>
        public async Task<OperationResultDto> resetPasswordWithCodeAsync(string email, string code, string newPassword)
        {
            return await executeServiceCallAsync(async (client) =>
                await client.resetPasswordWithCodeAsync(email, code, newPassword));
        }

        /// <summary>
        /// Logs out the user. 
        /// NOTE: Heartbeat should be stopped BEFORE calling this method.
        /// See SessionCleanupService for proper cleanup sequence.
        /// </summary>
        public async Task logoutAsync(string username)
        {
            await executeServiceCallAsync(async (client) =>
            {
                await client.logOutAsync(username);
                return 0;
            });
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // PRIVATE METHODS - Service Calls
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Executes a service call with proper client lifecycle management.
        /// </summary>
        private static async Task<T> executeServiceCallAsync<T>(Func<AuthenticationManagerClient, Task<T>> serviceCall)
        {
            var client = new AuthenticationManagerClient();

            try
            {
                T result = await serviceCall(client);
                closeClientSafe(client);
                return result;
            }
            catch (CommunicationException)
            {
                abortClientSafe(client);
                throw;
            }
            catch (TimeoutException)
            {
                abortClientSafe(client);
                throw;
            }
            catch (SocketException)
            {
                abortClientSafe(client);
                throw;
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // PRIVATE METHODS - Post-Login Service Initialization
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Safely connects the social service after login.
        /// </summary>
        private async Task<bool> connectSocialServiceSafeAsync(string username)
        {
            try
            {
                await socialService.connectAsync(username);
                System.Diagnostics.Debug.WriteLine("[AUTH] Social service connected");
                return true;
            }
            catch (CommunicationException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AUTH] Social service connection failed: {ex.Message}");
                return false;
            }
            catch (TimeoutException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AUTH] Social service connection timeout: {ex.Message}");
                return false;
            }
            catch (SocketException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AUTH] Social service socket error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Safely starts the heartbeat service after login.
        /// This is critical for maintaining connection health with the server.
        /// </summary>
        private async Task<bool> startHeartbeatServiceSafeAsync(string username)
        {
            try
            {
                bool started = await heartbeatService.startAsync(username);
                System.Diagnostics.Debug.WriteLine($"[AUTH] Heartbeat service started: {started}");
                return started;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AUTH] Error starting heartbeat: {ex.Message}");
                return false;
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // PRIVATE METHODS - Client Cleanup
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Safely closes the WCF client.
        /// </summary>
        private static void closeClientSafe(AuthenticationManagerClient client)
        {
            try
            {
                if (client.State == CommunicationState.Opened)
                {
                    client.Close();
                }
            }
            catch (CommunicationException)
            {
                client.Abort();
            }
            catch (TimeoutException)
            {
                client.Abort();
            }
        }

        /// <summary>
        /// Safely aborts the WCF client.
        /// </summary>
        private static void abortClientSafe(AuthenticationManagerClient client)
        {
            try
            {
                client.Abort();
            }
            catch
            {
                // Ignore
            }
        }
    }
}