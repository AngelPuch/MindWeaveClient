“
äC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Properties\AssemblyInfo.cs
[

 
assembly

 	
:

	 

AssemblyTitle

 
(

 
$str

 *
)

* +
]

+ ,
[ 
assembly 	
:	 

AssemblyDescription 
( 
$str !
)! "
]" #
[ 
assembly 	
:	 
!
AssemblyConfiguration  
(  !
$str! #
)# $
]$ %
[ 
assembly 	
:	 

AssemblyCompany 
( 
$str 
) 
] 
[ 
assembly 	
:	 

AssemblyProduct 
( 
$str ,
), -
]- .
[ 
assembly 	
:	 

AssemblyCopyright 
( 
$str 0
)0 1
]1 2
[ 
assembly 	
:	 

AssemblyTrademark 
( 
$str 
)  
]  !
[ 
assembly 	
:	 

AssemblyCulture 
( 
$str 
) 
] 
[ 
assembly 	
:	 


ComVisible 
( 
false 
) 
] 
["" 
assembly"" 	
:""	 

	ThemeInfo"" 
("" &
ResourceDictionaryLocation## 
.## 
None## #
,### $&
ResourceDictionaryLocation&& 
.&& 
SourceAssembly&& -
))) 
])) 
[33 
assembly33 	
:33	 

AssemblyVersion33 
(33 
$str33 $
)33$ %
]33% &
[44 
assembly44 	
:44	 

AssemblyFileVersion44 
(44 
$str44 (
)44( )
]44) *≤ä
{C:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\App.xaml.cs
	namespace 	
MindWeaveClient
 
{ 
public 

partial 
class 
App 
: 
Application *
{ 
	protected 
override 
void 
	OnStartup  )
() *
StartupEventArgs* :
e; <
)< =
{ 	
var 
langCode 
= 
MindWeaveClient *
.* +

Properties+ 5
.5 6
Settings6 >
.> ?
Default? F
.F G
languageCodeG S
;S T
Thread 
. 
CurrentThread  
.  !
CurrentUICulture! 1
=2 3
new4 7
System8 >
.> ?
Globalization? L
.L M
CultureInfoM X
(X Y
langCodeY a
)a b
;b c
base 
. 
	OnStartup 
( 
e 
) 
; 
try 
{ 
AudioManager 
. 

Initialize '
(' (
)( )
;) *
Debug 
. 
	WriteLine 
(  
$str  J
)J K
;K L
} 
catch 
( 
	Exception 
ex 
)  
{ 
Debug   
.   
	WriteLine   
(    
$"    "
$str  " T
{  T U
ex  U W
.  W X
Message  X _
}  _ `
"  ` a
)  a b
;  b c
}## 
Debug%% 
.%% 
	WriteLine%% 
(%% 
$str%% 6
)%%6 7
;%%7 8
}&& 	
public)) 
static)) 
void)) $
SubscribeToGlobalInvites)) 3
())3 4
)))4 5
{** 	
if++ 
(++ &
SocialServiceClientManager++ *
.++* +
instance+++ 3
.++3 4
proxy++4 9
?++9 :
.++: ;
State++; @
==++A C
System++D J
.++J K
ServiceModel++K W
.++W X
CommunicationState++X j
.++j k
Opened++k q
&&++r t&
SocialServiceClientManager,, *
.,,* +
instance,,+ 3
.,,3 4
callbackHandler,,4 C
!=,,D F
null,,G K
),,K L
{-- &
SocialServiceClientManager.. *
...* +
instance..+ 3
...3 4
callbackHandler..4 C
...C D
LobbyInviteReceived..D W
-=..X Z#
App_LobbyInviteReceived..[ r
;..r s&
SocialServiceClientManager// *
.//* +
instance//+ 3
.//3 4
callbackHandler//4 C
.//C D
LobbyInviteReceived//D W
+=//X Z#
App_LobbyInviteReceived//[ r
;//r s
Debug00 
.00 
	WriteLine00 
(00  
$str00  r
)00r s
;00s t
}11 
else22 
{33 
Debug44 
.44 
	WriteLine44 
(44  
$"44  "
$str44" p
{44p q'
SocialServiceClientManager	44q ã
.
44ã å
instance
44å î
.
44î ï
proxy
44ï ö
?
44ö õ
.
44õ ú
State
44ú °
}
44° ¢
$str
44¢ ∫
{
44∫ ª(
SocialServiceClientManager
44ª ’
.
44’ ÷
instance
44÷ ﬁ
.
44ﬁ ﬂ
callbackHandler
44ﬂ Ó
==
44Ô Ò
null
44Ú ˆ
}
44ˆ ˜
"
44˜ ¯
)
44¯ ˘
;
44˘ ˙
if55 
(55 
Application55 
.55  
Current55  '
.55' (

MainWindow55( 2
!=553 5
null556 :
&&55; =
Application55> I
.55I J
Current55J Q
.55Q R

MainWindow55R \
.55\ ]
IsLoaded55] e
)55e f
{66 

MessageBox77 
.77 
Show77 #
(77# $
$str77$ ]
,77] ^
$str77_ f
,77f g
MessageBoxButton77h x
.77x y
OK77y {
,77{ |
MessageBoxImage	77} å
.
77å ç
Warning
77ç î
)
77î ï
;
77ï ñ
}88 
}99 
}:: 	
private== 
static== 
void== #
App_LobbyInviteReceived== 3
(==3 4
string==4 :
fromUsername==; G
,==G H
string==I O
lobbyId==P W
)==W X
{>> 	
Debug?? 
.?? 
	WriteLine?? 
(?? 
$"?? 
$str?? C
{??C D
fromUsername??D P
}??P Q
$str??Q \
{??\ ]
lobbyId??] d
}??d e
$str??e u
{??u v
SessionService	??v Ñ
.
??Ñ Ö
Username
??Ö ç
}
??ç é
"
??é è
)
??è ê
;
??ê ë
ifAA 
(AA 
fromUsernameAA 
.AA 
EqualsAA #
(AA# $
SessionServiceAA$ 2
.AA2 3
UsernameAA3 ;
,AA; <
StringComparisonAA= M
.AAM N
OrdinalIgnoreCaseAAN _
)AA_ `
)AA` a
{BB 
DebugBB 
.BB 
	WriteLineBB 
(BB 
$strBB M
)BBM N
;BBN O
returnBBP V
;BBV W
}BBX Y
MessageBoxResultDD 
resultDD #
=DD$ %

MessageBoxDD& 0
.DD0 1
ShowDD1 5
(DD5 6
$"EE 
$strEE 7
{EE7 8
fromUsernameEE8 D
}EED E
$strEEE T
{EET U
lobbyIdEEU \
}EE\ ]
$strEE] u
"EEu v
,EEv w
$strFF "
,FF" #
MessageBoxButtonGG  
.GG  !
YesNoGG! &
,GG& '
MessageBoxImageHH 
.HH  
QuestionHH  (
)HH( )
;HH) *
ifJJ 
(JJ 
resultJJ 
==JJ 
MessageBoxResultJJ *
.JJ* +
YesJJ+ .
)JJ. /
{KK 
DebugLL 
.LL 
	WriteLineLL 
(LL  
$"LL  "
$strLL" '
{LL' (
SessionServiceLL( 6
.LL6 7
UsernameLL7 ?
}LL? @
$strLL@ Z
{LLZ [
lobbyIdLL[ b
}LLb c
$str	LLc á
"
LLá à
)
LLà â
;
LLâ ä
ifMM 
(MM +
MatchmakingServiceClientManagerMM 3
.MM3 4
instanceMM4 <
.MM< =
EnsureConnectedMM= L
(MML M
)MMM N
)MMN O
{NN 
DebugOO 
.OO 
	WriteLineOO #
(OO# $
$strOO$ Y
)OOY Z
;OOZ [
tryPP 
{QQ 
varRR 
matchmakingProxyRR ,
=RR- .+
MatchmakingServiceClientManagerRR/ N
.RRN O
instanceRRO W
.RRW X
proxyRRX ]
;RR] ^
ifSS 
(SS 
stringSS "
.SS" #
IsNullOrEmptySS# 0
(SS0 1
SessionServiceSS1 ?
.SS? @
UsernameSS@ H
)SSH I
)SSI J
{TT 
DebugUU !
.UU! "
	WriteLineUU" +
(UU+ ,
$strUU, o
)UUo p
;UUp q

MessageBoxVV &
.VV& '
ShowVV' +
(VV+ ,
$strVV, O
,VVO P
LangVVQ U
.VVU V

ErrorTitleVVV `
)VV` a
;VVa b
returnWW "
;WW" #
}XX 
matchmakingProxyYY (
.YY( )
	joinLobbyYY) 2
(YY2 3
SessionServiceYY3 A
.YYA B
UsernameYYB J
,YYJ K
lobbyIdYYL S
)YYS T
;YYT U
Debug[[ 
.[[ 
	WriteLine[[ '
([[' (
$"[[( *
$str[[* L
{[[L M
lobbyId[[M T
}[[T U
$str[[U X
"[[X Y
)[[Y Z
;[[Z [
NavigateToLobbyPage\\ +
(\\+ ,
)\\, -
;\\- .
}]] 
catch^^ 
(^^ 
	Exception^^ $
ex^^% '
)^^' (
{__ 
Debug`` 
.`` 
	WriteLine`` '
(``' (
$"``( *
$str``* T
{``T U
ex``U W
}``W X
"``X Y
)``Y Z
;``Z [

MessageBoxaa "
.aa" #
Showaa# '
(aa' (
$"aa( *
$straa* E
{aaE F
lobbyIdaaF M
}aaM N
$straaN P
{aaP Q
exaaQ S
.aaS T
MessageaaT [
}aa[ \
"aa\ ]
,aa] ^
Langaa_ c
.aac d

ErrorTitleaad n
,aan o
MessageBoxButton	aap Ä
.
aaÄ Å
OK
aaÅ É
,
aaÉ Ñ
MessageBoxImage
aaÖ î
.
aaî ï
Error
aaï ö
)
aaö õ
;
aaõ ú+
MatchmakingServiceClientManagerbb 7
.bb7 8
instancebb8 @
.bb@ A

DisconnectbbA K
(bbK L
)bbL M
;bbM N
}cc 
}dd 
elseee 
{ff 
Debuggg 
.gg 
	WriteLinegg #
(gg# $
$strgg$ O
)ggO P
;ggP Q

MessageBoxhh 
.hh 
Showhh #
(hh# $
Langhh$ (
.hh( )$
CannotConnectMatchmakinghh) A
,hhA B
LanghhC G
.hhG H

ErrorTitlehhH R
,hhR S
MessageBoxButtonhhT d
.hhd e
OKhhe g
,hhg h
MessageBoxImagehhi x
.hhx y
Warning	hhy Ä
)
hhÄ Å
;
hhÅ Ç
}ii 
}jj 
elsekk 
{ll 
Debugmm 
.mm 
	WriteLinemm 
(mm  
$"mm  "
$strmm" '
{mm' (
SessionServicemm( 6
.mm6 7
Usernamemm7 ?
}mm? @
$strmm@ Z
{mmZ [
lobbyIdmm[ b
}mmb c
$strmmc d
"mmd e
)mme f
;mmf g
}nn 
}oo 	
privaterr 
staticrr 
voidrr 
NavigateToLobbyPagerr /
(rr/ 0
)rr0 1
{ss 	
Applicationuu 
.uu 
Currentuu 
.uu  

Dispatcheruu  *
.uu* +
Invokeuu+ 1
(uu1 2
(uu2 3
)uu3 4
=>uu5 7
{uu8 9

MainWindowww 

mainWindowww %
=ww& '
Applicationww( 3
.ww3 4
Currentww4 ;
.ww; <
Windowsww< C
.wwC D
OfTypewwD J
<wwJ K

MainWindowwwK U
>wwU V
(wwV W
)wwW X
.wwX Y
FirstOrDefaultwwY g
(wwg h
)wwh i
;wwi j
ifzz 
(zz 

mainWindowzz 
!=zz !
nullzz" &
&&zz' )

mainWindowzz* 4
.zz4 5
	MainFramezz5 >
?zz> ?
.zz? @
NavigationServicezz@ Q
!=zzR T
nullzzU Y
)zzY Z
{{{ 
Debug|| 
.|| 
	WriteLine|| #
(||# $
$str||$ v
)||v w
;||w x
while 
( 

mainWindow %
.% &
	MainFrame& /
./ 0
NavigationService0 A
.A B
	CanGoBackB K
)K L
{
ÄÄ 

mainWindow
ÅÅ "
.
ÅÅ" #
	MainFrame
ÅÅ# ,
.
ÅÅ, -
NavigationService
ÅÅ- >
.
ÅÅ> ?
RemoveBackEntry
ÅÅ? N
(
ÅÅN O
)
ÅÅO P
;
ÅÅP Q
}
ÇÇ 
var
ÖÖ 
	lobbyPage
ÖÖ !
=
ÖÖ" #
new
ÖÖ$ '
	LobbyPage
ÖÖ( 1
(
ÖÖ1 2
)
ÖÖ2 3
;
ÖÖ3 4
	lobbyPage
ÜÜ 
.
ÜÜ 
DataContext
ÜÜ )
=
ÜÜ* +
new
ÜÜ, /
LobbyViewModel
ÜÜ0 >
(
ÜÜ> ?
null
áá 
,
áá 
page
àà 
=>
àà 

mainWindow
àà  *
.
àà* +
	MainFrame
àà+ 4
.
àà4 5
Navigate
àà5 =
(
àà= >
page
àà> B
)
ààB C
,
ààC D
(
ää 
)
ää 
=>
ää 

mainWindow
ää (
.
ää( )
	MainFrame
ää) 2
.
ää2 3
Navigate
ää3 ;
(
ää; <
new
ää< ?
MainMenuPage
ää@ L
(
ääL M
page
ääM Q
=>
ääR T

mainWindow
ääU _
.
ää_ `
	MainFrame
ää` i
.
ääi j
Navigate
ääj r
(
äär s
page
ääs w
)
ääw x
)
ääx y
)
ääy z
)
ãã 
;
ãã 

mainWindow
éé 
.
éé 
	MainFrame
éé (
.
éé( )
Navigate
éé) 1
(
éé1 2
	lobbyPage
éé2 ;
)
éé; <
;
éé< =
Debug
èè 
.
èè 
	WriteLine
èè #
(
èè# $
$str
èè$ J
)
èèJ K
;
èèK L
}
êê 
else
ëë 
{
íí 
string
îî 
errorReason
îî &
=
îî' (
$str
îî) 9
;
îî9 :
if
ïï 
(
ïï 

mainWindow
ïï "
==
ïï# %
null
ïï& *
)
ïï* +
{
ññ 
errorReason
óó #
=
óó$ %
$str
óó& x
;
óóx y
var
ôô 
openWindowTypes
ôô +
=
ôô, -
string
ôô. 4
.
ôô4 5
Join
ôô5 9
(
ôô9 :
$str
ôô: >
,
ôô> ?
Application
ôô@ K
.
ôôK L
Current
ôôL S
.
ôôS T
Windows
ôôT [
.
ôô[ \
OfType
ôô\ b
<
ôôb c
Window
ôôc i
>
ôôi j
(
ôôj k
)
ôôk l
.
ôôl m
Select
ôôm s
(
ôôs t
w
ôôt u
=>
ôôv x
w
ôôy z
.
ôôz {
GetTypeôô{ Ç
(ôôÇ É
)ôôÉ Ñ
.ôôÑ Ö
NameôôÖ â
)ôôâ ä
)ôôä ã
;ôôã å
Debug
öö 
.
öö 
	WriteLine
öö '
(
öö' (
$"
öö( *
$str
öö* 9
{
öö9 :
openWindowTypes
öö: I
}
ööI J
$str
ööJ K
"
ööK L
)
ööL M
;
ööM N
}
õõ 
else
úú 
if
úú 
(
úú 

mainWindow
úú '
.
úú' (
	MainFrame
úú( 1
==
úú2 4
null
úú5 9
)
úú9 :
{
úú; <
errorReason
úú= H
=
úúI J
$strúúK Ç
;úúÇ É
}úúÑ Ö
else
ùù 
if
ùù 
(
ùù 

mainWindow
ùù '
.
ùù' (
	MainFrame
ùù( 1
.
ùù1 2
NavigationService
ùù2 C
==
ùùD F
null
ùùG K
)
ùùK L
{
ùùM N
errorReason
ùùO Z
=
ùù[ \
$strùù] ß
;ùùß ®
}ùù© ™
Debug
üü 
.
üü 
	WriteLine
üü #
(
üü# $
$"
üü$ &
$str
üü& w
{
üüw x
errorReasonüüx É
}üüÉ Ñ
"üüÑ Ö
)üüÖ Ü
;üüÜ á

MessageBox
†† 
.
†† 
Show
†† #
(
††# $
$"
††$ &
$str
††& d
{
††d e
errorReason
††e p
}
††p q
$str
††q r
"
††r s
,
††s t
Lang
††u y
.
††y z

ErrorTitle††z Ñ
)††Ñ Ö
;††Ö Ü
}
°° 
}
¢¢ 
)
¢¢ 
;
¢¢ 
}
££ 	
	protected
¶¶ 
override
¶¶ 
void
¶¶ 
OnExit
¶¶  &
(
¶¶& '
ExitEventArgs
¶¶' 4
e
¶¶5 6
)
¶¶6 7
{
ßß 	
AudioManager
®® 
.
®® 
	stopMusic
®® "
(
®®" #
)
®®# $
;
®®$ %
if
™™ 
(
™™ (
SocialServiceClientManager
™™ *
.
™™* +
instance
™™+ 3
.
™™3 4
callbackHandler
™™4 C
!=
™™D F
null
™™G K
)
™™K L
{
´´ (
SocialServiceClientManager
¨¨ *
.
¨¨* +
instance
¨¨+ 3
.
¨¨3 4
callbackHandler
¨¨4 C
.
¨¨C D!
LobbyInviteReceived
¨¨D W
-=
¨¨X Z%
App_LobbyInviteReceived
¨¨[ r
;
¨¨r s
Debug
≠≠ 
.
≠≠ 
	WriteLine
≠≠ 
(
≠≠  
$str
≠≠  b
)
≠≠b c
;
≠≠c d
}
ÆÆ (
SocialServiceClientManager
±± &
.
±±& '
instance
±±' /
.
±±/ 0

Disconnect
±±0 :
(
±±: ;
)
±±; <
;
±±< =-
MatchmakingServiceClientManager
≤≤ +
.
≤≤+ ,
instance
≤≤, 4
.
≤≤4 5

Disconnect
≤≤5 ?
(
≤≤? @
)
≤≤@ A
;
≤≤A B&
ChatServiceClientManager
≥≥ $
.
≥≥$ %
instance
≥≥% -
.
≥≥- .

Disconnect
≥≥. 8
(
≥≥8 9
)
≥≥9 :
;
≥≥: ;
base
µµ 
.
µµ 
OnExit
µµ 
(
µµ 
e
µµ 
)
µµ 
;
µµ 
}
∂∂ 	
}
∑∑ 
}∏∏ ∞
êC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\View\Main\SettingsWindow.xaml.cs
	namespace 	
MindWeaveClient
 
. 
View 
. 
Settings '
{ 
public		 

partial		 
class		 
SettingsWindow		 '
:		( )
Window		* 0
{

 
public 
SettingsWindow 
( 
) 
{ 	
InitializeComponent 
(  
)  !
;! "
this 
. 
DataContext 
= 
new "
SettingsViewModel# 4
(4 5
this5 9
)9 :
;: ;
} 	
} 
} Ò
åC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\View\Main\SocialPage.xaml.cs
	namespace 	
MindWeaveClient
 
. 
View 
. 
Main #
{ 
public 

partial 
class 

SocialPage #
:$ %
Page& *
{ 
private		 
SocialViewModel		 

_viewModel		  *
;		* +
public 

SocialPage 
( 
) 
{ 	
InitializeComponent 
(  
)  !
;! "

_viewModel 
= 
new 
SocialViewModel ,
(, -
(- .
). /
=>0 2
NavigationService3 D
?D E
.E F
GoBackF L
(L M
)M N
)N O
;O P
DataContext 
= 

_viewModel $
;$ %
Unloaded 
+= 
SocialPage_Unloaded +
;+ ,
} 	
private 
void 
SocialPage_Unloaded (
(( )
object) /
sender0 6
,6 7
System8 >
.> ?
Windows? F
.F G
RoutedEventArgsG V
eW X
)X Y
{ 	

_viewModel 
? 
. 
cleanup 
(  
)  !
;! "
Unloaded 
-= 
SocialPage_Unloaded +
;+ ,
} 	
private 
void !
SearchTextBox_KeyDown *
(* +
object+ 1
sender2 8
,8 9
KeyEventArgs: F
eG H
)H I
{ 	
if 
( 
e 
. 
Key 
== 
Key 
. 
Enter "
&&# %

_viewModel& 0
.0 1
SearchCommand1 >
.> ?

CanExecute? I
(I J
nullJ N
)N O
)O P
{ 

_viewModel 
. 
SearchCommand (
.( )
Execute) 0
(0 1
null1 5
)5 6
;6 7
} 
}   	
}!! 
}"" Å
ïC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\View\Main\SelectionPuzzlePage.xaml.cs
	namespace 	
MindWeaveClient
 
. 
View 
. 
Main #
{ 
public 

partial 
class 
SelectionPuzzlePage ,
:- .
Page/ 3
{ 
public 
SelectionPuzzlePage "
(" #
)# $
{ 	
InitializeComponent 
(  
)  !
;! "
} 	
} 
} ©
íC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\View\Main\SelectAvatarPage.xaml.cs
	namespace 	
MindWeaveClient
 
. 
View 
. 
Main #
{ 
public 

partial 
class 
SelectAvatarPage )
:* +
Page, 0
{ 
public		 
SelectAvatarPage		 
(		  
)		  !
{

 	
InitializeComponent 
(  
)  !
;! "
} 	
private 
void 
OnAvatarSelected %
(% &
object& ,
sender- 3
,3 4
RoutedEventArgs5 D
eE F
)F G
{ 	
if 
( 
sender 
is 
FrameworkElement *
element+ 2
&&3 5
element6 =
.= >
DataContext> I
isJ L
AvatarM S
avatarT Z
)Z [
{ 
if 
( 
element 
. 
Tag 
is  "!
SelectAvatarViewModel# 8
	viewModel9 B
)B C
{ 
	viewModel 
. 
SelectedAvatar ,
=- .
avatar/ 5
;5 6
} 
else 
{ 
if 
( 
this 
. 
DataContext (
is) +!
SelectAvatarViewModel, A
vmB D
)D E
{ 
vm 
. 
SelectedAvatar )
=* +
avatar, 2
;2 3
} 
}   
}!! 
}"" 	
}## 
}$$ È
çC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\View\Main\ProfilePage.xaml.cs
	namespace 	
MindWeaveClient
 
. 
View 
. 
Main #
{ 
public		 

partial		 
class		 
ProfilePage		 $
:		% &
Page		' +
{

 
public 
ProfilePage 
( 
) 
{ 	
InitializeComponent 
(  
)  !
;! "
} 	
} 
} ú
åC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\View\Main\MainWindow.xaml.cs
	namespace

 	
MindWeaveClient


 
.

 
View

 
.

 
Main

 #
{ 
public 

partial 
class 

MainWindow #
:$ %
Window& ,
{ 
public 

MainWindow 
( 
) 
{ 	
InitializeComponent 
(  
)  !
;! "
	MainFrame 
. 
Navigate 
( 
new "
MainMenuPage# /
(/ 0
page0 4
=>5 7
	MainFrame8 A
.A B
NavigateB J
(J K
pageK O
)O P
)P Q
)Q R
;R S
this 
. 
Loaded 
+= 
MainWindow_Loaded ,
;, -
this 
. 
Closed 
+= 
MainWindow_Closed ,
;, -
} 	
private 
void 
MainWindow_Loaded &
(& '
object' -
sender. 4
,4 5
RoutedEventArgs6 E
eF G
)G H
{ 	
Debug 
. 
	WriteLine 
( 
$str ]
)] ^
;^ _
App 
. $
SubscribeToGlobalInvites (
(( )
)) *
;* +
}   	
private"" 
void"" 
MainWindow_Closed"" &
(""& '
object""' -
sender"". 4
,""4 5
	EventArgs""6 ?
e""@ A
)""A B
{## 	
if%% 
(%% &
SocialServiceClientManager%% *
.%%* +
instance%%+ 3
.%%3 4
callbackHandler%%4 C
!=%%D F
null%%G K
)%%K L
{&& 
Debug** 
.** 
	WriteLine** 
(**  
$str**  l
)**l m
;**m n
}++ 
}00 	
}11 
}22 Ü
éC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\View\Main\MainMenuPage.xaml.cs
	namespace 	
MindWeaveClient
 
. 
View 
. 
Main #
{ 
public

 

partial

 
class

 
MainMenuPage

 %
:

& '
Page

( ,
{ 
public 
MainMenuPage 
( 
Action "
<" #
Page# '
>' (

navigateTo) 3
)3 4
{ 	
InitializeComponent 
(  
)  !
;! "
DataContext 
= 
new 
MainMenuViewModel /
(/ 0

navigateTo0 :
,: ;
this< @
)@ A
;A B
} 	
} 
} Ü

ëC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\View\Main\EditProfilePage.xaml.cs
	namespace 	
MindWeaveClient
 
. 
View 
. 
Main #
{ 
public 

partial 
class 
EditProfilePage (
:) *
Page+ /
{ 
public 
EditProfilePage 
( 
)  
{ 	
InitializeComponent 
(  
)  !
;! "
this 
. 
Loaded 
+= "
EditProfilePage_Loaded 1
;1 2
} 	
private 
void "
EditProfilePage_Loaded +
(+ ,
object, 2
sender3 9
,9 :
RoutedEventArgs; J
eK L
)L M
{   	
if## 
(## 
this## 
.## 
DataContext##  
is##! # 
EditProfileViewModel##$ 8
vm##9 ;
)##; <
{$$ 
vm(( 
.(( 
RefreshAvatar((  
(((  !
)((! "
;((" #
})) 
}** 	
}++ 
},, Ê

ãC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\View\Game\LobbyPage.xaml.cs
	namespace 	
MindWeaveClient
 
. 
View 
. 
Game #
{ 
public 

partial 
class 
	LobbyPage "
:# $
Page% )
{		 
public

 
	LobbyPage

 
(

 
)

 
{ 	
InitializeComponent 
(  
)  !
;! "
this 
. 
Unloaded 
+= 
LobbyPage_Unloaded /
;/ 0
} 	
private 
void 
LobbyPage_Unloaded '
(' (
object( .
sender/ 5
,5 6
RoutedEventArgs7 F
eG H
)H I
{ 	
if 
( 
this 
. 
DataContext  
is! #
LobbyViewModel$ 2
	viewModel3 <
)< =
{ 
	viewModel 
. 
cleanup !
(! "
)" #
;# $
} 
this 
. 
Unloaded 
-= 
LobbyPage_Unloaded /
;/ 0
} 	
} 
} Ë
åC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\View\Game\GameWindow.xaml.cs
	namespace 	
MindWeaveClient
 
. 
View 
. 
Game #
{ 
public 

partial 
class 

GameWindow #
:$ %
Window& ,
{ 
public 

GameWindow 
( 
) 
{ 	
InitializeComponent 
(  
)  !
;! "
} 	
} 
} ∫
ïC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\View\Dialogs\GuestInputDialog.xaml.cs
	namespace 	
MindWeaveClient
 
. 
View 
. 
Dialogs &
{ 
public 

partial 
class 
GuestInputDialog )
:* +
Window, 2
{ 
public 
string 

GuestEmail  
{! "
get# &
;& '
private( /
set0 3
;3 4
}5 6
public

 
GuestInputDialog

 
(

  
)

  !
{ 	
InitializeComponent 
(  
)  !
;! "
Loaded 
+= 
( 
s 
, 
e 
) 
=> 
EmailTextBox  ,
., -
Focus- 2
(2 3
)3 4
;4 5
} 	
private 
void 
OkButton_Click #
(# $
object$ *
sender+ 1
,1 2
RoutedEventArgs3 B
eC D
)D E
{ 	

GuestEmail 
= 
EmailTextBox %
.% &
Text& *
;* +
this 
. 
DialogResult 
= 
true  $
;$ %
this 
. 
Close 
( 
) 
; 
} 	
} 
} ã
úC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\View\Authentication\VerificationPage.xaml.cs
	namespace 	
MindWeaveClient
 
. 
View 
. 
Authentication -
{ 
public 

partial 
class 
VerificationPage )
:* +
Page, 0
{ 
public 
VerificationPage 
(  
string  &
email' ,
), -
{ 	
InitializeComponent 
(  
)  !
;! "
this 
. 
DataContext 
= 
new "!
VerificationViewModel# 8
(8 9
email 
, 
page 
=> 
this 
. 
NavigationService .
?. /
./ 0
Navigate0 8
(8 9
page9 =
)= >
,> ?
( 
) 
=> 
{ 
if 
( 
this  
.  !
NavigationService! 2
.2 3
	CanGoBack3 <
)< =
this> B
.B C
NavigationServiceC T
.T U
GoBackU [
([ \
)\ ]
;] ^
}_ `
)   
;   
}!! 	
private## 
void## (
CodeTextBox_PreviewTextInput## 1
(##1 2
object##2 8
sender##9 ?
,##? @$
TextCompositionEventArgs##A Y
e##Z [
)##[ \
{$$ 	
Regex%% 
regex%% 
=%% 
new%% 
Regex%% #
(%%# $
$str%%$ -
)%%- .
;%%. /
e&& 
.&& 
Handled&& 
=&& 
regex&& 
.&& 
IsMatch&& %
(&&% &
e&&& '
.&&' (
Text&&( ,
)&&, -
;&&- .
}'' 	
private)) 
void)) 
Button_Click)) !
())! "
object))" (
sender))) /
,))/ 0
RoutedEventArgs))1 @
e))A B
)))B C
{** 	
},, 	
}-- 
}.. ≤
†C:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\View\Authentication\PasswordRecoveryPage.xaml.cs
	namespace

 	
MindWeaveClient


 
.

 
View

 
.

 
Authentication

 -
{ 
public 

partial 
class  
PasswordRecoveryPage -
:. /
Page0 4
{ 
public  
PasswordRecoveryPage #
(# $
)$ %
{ 	
InitializeComponent 
(  
)  !
;! "
this 
. 
DataContext 
= 
new "%
PasswordRecoveryViewModel# <
(< =
( 
) 
=> 
{ 
if 
( 
this  
.  !
NavigationService! 2
.2 3
	CanGoBack3 <
)< =
this> B
.B C
NavigationServiceC T
.T U
GoBackU [
([ \
)\ ]
;] ^
}_ `
,` a
( 
) 
=> 
this 
. 
NavigationService ,
?, -
.- .
Navigate. 6
(6 7
new7 :
	LoginPage; D
(D E
)E F
)F G
) 
; 
} 	
private 
void (
CodeTextBox_PreviewTextInput 1
(1 2
object2 8
sender9 ?
,? @$
TextCompositionEventArgsA Y
eZ [
)[ \
{ 	
Regex 
regex 
= 
new 
Regex #
(# $
$str$ -
)- .
;. /
e 
. 
Handled 
= 
regex 
. 
IsMatch %
(% &
e& '
.' (
Text( ,
), -
;- .
} 	
}!! 
}"" ‡
ïC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\View\Authentication\LoginPage.xaml.cs
	namespace 	
MindWeaveClient
 
. 
View 
. 
Authentication -
{ 
public 

partial 
class 
	LoginPage "
:# $
Page% )
{ 
public		 
	LoginPage		 
(		 
)		 
{

 	
InitializeComponent 
(  
)  !
;! "
this 
. 
DataContext 
= 
new "
LoginViewModel# 1
(1 2
page2 6
=>7 9
this: >
.> ?
NavigationService? P
?P Q
.Q R
NavigateR Z
(Z [
page[ _
)_ `
)` a
;a b
} 	
} 
} Â
ôC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\View\Authentication\GuestJoinPage.xaml.cs
	namespace 	
MindWeaveClient
 
. 
View 
. 
Authentication -
{ 
public		 

partial		 
class		 
GuestJoinPage		 &
:		' (
Page		) -
{

 
public 
GuestJoinPage 
( 
) 
{ 	
InitializeComponent 
(  
)  !
;! "
this 
. 
DataContext 
= 
new "
GuestJoinViewModel# 5
(5 6
page 
=> 
this 
. 
NavigationService .
?. /
./ 0
Navigate0 8
(8 9
page9 =
)= >
,> ?
( 
) 
=> 
{ 
if 
( 
this  
.  !
NavigationService! 2
.2 3
	CanGoBack3 <
)< =
this> B
.B C
NavigationServiceC T
.T U
GoBackU [
([ \
)\ ]
;] ^
}_ `
) 
; 
} 	
private 
void (
CodeTextBox_PreviewTextInput 1
(1 2
object2 8
sender9 ?
,? @$
TextCompositionEventArgsA Y
eZ [
)[ \
{ 	
Regex 
regex 
= 
new 
Regex #
(# $
$str$ 4
)4 5
;5 6
e 
. 
Handled 
= 
! 
regex 
. 
IsMatch &
(& '
e' (
.( )
Text) -
)- .
;. /
} 	
} 
} Ä
ùC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\View\Authentication\CreateAccountPage.xaml.cs
	namespace 	
MindWeaveClient
 
. 
View 
. 
Authentication -
{ 
public		 

partial		 
class		 
CreateAccountPage		 *
:		+ ,
Page		- 1
{

 
public 
CreateAccountPage  
(  !
)! "
{ 	
InitializeComponent 
(  
)  !
;! "
this 
. 
DataContext 
= 
new ""
CreateAccountViewModel# 9
(9 :
page: >
=>? A
thisB F
.F G
NavigationServiceG X
?X Y
.Y Z
NavigateZ b
(b c
pagec g
)g h
)h i
;i j
} 	
} 
} ”
†C:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\View\Authentication\AuthenticationWindow.xaml.cs
	namespace 	
MindWeaveClient
 
. 
View 
. 
Authentication -
{ 
public 

partial 
class  
AuthenticationWindow -
:. /
Window0 6
{ 
public  
AuthenticationWindow #
(# $
)$ %
{ 	
InitializeComponent 
(  
)  !
;! "
AuthenticationFrame 
.  
Navigate  (
(( )
new) ,
	LoginPage- 6
(6 7
)7 8
)8 9
;9 :
} 	
} 
} —
âC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\ViewModel\RelayCommand.cs
	namespace 	
MindWeaveClient
 
. 
	ViewModel #
{ 
public 

class 
RelayCommand 
: 
ICommand  (
{ 
private 
readonly 
Action 
<  
object  &
>& '
execute( /
;/ 0
private		 
readonly		 
	Predicate		 "
<		" #
object		# )
>		) *

canExecute		+ 5
;		5 6
public 
RelayCommand 
( 
Action "
<" #
object# )
>) *
execute+ 2
,2 3
	Predicate4 =
<= >
object> D
>D E

canExecuteF P
=Q R
nullS W
)W X
{ 	
this 
. 
execute 
= 
execute "
??# %
throw& +
new, /!
ArgumentNullException0 E
(E F
nameofF L
(L M
executeM T
)T U
)U V
;V W
this 
. 

canExecute 
= 

canExecute (
;( )
} 	
public 
event 
EventHandler !
CanExecuteChanged" 3
{ 	
add 
{ 
CommandManager  
.  !
RequerySuggested! 1
+=2 4
value5 :
;: ;
}< =
remove 
{ 
CommandManager #
.# $
RequerySuggested$ 4
-=5 7
value8 =
;= >
}? @
} 	
public 
bool 

CanExecute 
( 
object %
	parameter& /
)/ 0
{ 	
return 

canExecute 
==  
null! %
||& (

canExecute) 3
(3 4
	parameter4 =
)= >
;> ?
} 	
public 
void 
Execute 
( 
object "
	parameter# ,
), -
{ 	
execute 
( 
	parameter 
) 
; 
} 	
public!! 
void!! "
RaiseCanExecuteChanged!! *
(!!* +
)!!+ ,
{"" 	
CommandManager## 
.## &
InvalidateRequerySuggested## 5
(##5 6
)##6 7
;##7 8
}$$ 	
}%% 
}&& Óù
ëC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\ViewModel\Main\SocialViewModel.cs
	namespace 	
MindWeaveClient
 
. 
	ViewModel #
.# $
Main$ (
{ 
public 

class 
FriendDtoDisplay !
:" #
BaseViewModel$ 1
{ 
private 
bool 
isOnlineValue "
;" #
public 
string 
Username 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
string 

AvatarPath  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
bool 
IsOnline 
{ 	
get 
=> 
isOnlineValue  
;  !
set 
{ 
isOnlineValue 
=  !
value" '
;' (
OnPropertyChanged) :
(: ;
); <
;< =
}> ?
} 	
public 
FriendDtoDisplay 
(  
	FriendDto  )
dto* -
)- .
{ 	
this 
. 
Username 
= 
dto 
.  
username  (
;( )
this 
. 

AvatarPath 
= 
dto !
.! "

avatarPath" ,
??- /
$str0 ]
;] ^
this 
. 
IsOnline 
= 
dto 
.  
isOnline  (
;( )
} 	
public 
FriendDtoDisplay 
(  
)  !
{" #
}$ %
}   
public"" 

class"" 
SocialViewModel""  
:""! "
BaseViewModel""# 0
{## 
private$$ 
SocialManagerClient$$ #
proxy$$$ )
=>$$* ,&
SocialServiceClientManager$$- G
.$$G H
instance$$H P
.$$P Q
proxy$$Q V
;$$V W
private%% !
SocialCallbackHandler%% %
callbackHandler%%& 5
=>%%6 8&
SocialServiceClientManager%%9 S
.%%S T
instance%%T \
.%%\ ]
callbackHandler%%] l
;%%l m
private'' 
string'' 
searchQueryValue'' '
;''' (
private(( 
bool(( %
isFriendsListCheckedValue(( .
=((/ 0
true((1 5
;((5 6
private)) 
bool)) #
isAddFriendCheckedValue)) ,
;)), -
private** 
bool** "
isRequestsCheckedValue** +
;**+ ,
private++ 
bool++ 
isBusyValue++  
;++  !
public--  
ObservableCollection-- #
<--# $
FriendDtoDisplay--$ 4
>--4 5
FriendsList--6 A
{--B C
get--D G
;--G H
}--I J
=--K L
new--M P 
ObservableCollection--Q e
<--e f
FriendDtoDisplay--f v
>--v w
(--w x
)--x y
;--y z
public..  
ObservableCollection.. #
<..# $!
PlayerSearchResultDto..$ 9
>..9 :
SearchResults..; H
{..I J
get..K N
;..N O
}..P Q
=..R S
new..T W 
ObservableCollection..X l
<..l m"
PlayerSearchResultDto	..m Ç
>
..Ç É
(
..É Ñ
)
..Ñ Ö
;
..Ö Ü
public//  
ObservableCollection// #
<//# $ 
FriendRequestInfoDto//$ 8
>//8 9
ReceivedRequests//: J
{//K L
get//M P
;//P Q
}//R S
=//T U
new//V Y 
ObservableCollection//Z n
<//n o!
FriendRequestInfoDto	//o É
>
//É Ñ
(
//Ñ Ö
)
//Ö Ü
;
//Ü á
public11 
string11 
SearchQuery11 !
{11" #
get11$ '
=>11( *
searchQueryValue11+ ;
;11; <
set11= @
{11A B
searchQueryValue11C S
=11T U
value11V [
;11[ \
OnPropertyChanged11] n
(11n o
)11o p
;11p q
}11r s
}11t u
public22 
bool22  
IsFriendsListChecked22 (
{22) *
get22+ .
=>22/ 1%
isFriendsListCheckedValue222 K
;22K L
set22M P
{22Q R%
isFriendsListCheckedValue22S l
=22m n
value22o t
;22t u
OnPropertyChanged	22v á
(
22á à
)
22à â
;
22â ä
if
22ã ç
(
22é è
value
22è î
)
22î ï$
LoadFriendsListCommand
22ñ ¨
.
22¨ ≠
Execute
22≠ ¥
(
22¥ µ
null
22µ π
)
22π ∫
;
22∫ ª
}
22º Ω
}
22æ ø
public33 
bool33 
IsAddFriendChecked33 &
{33' (
get33) ,
=>33- /#
isAddFriendCheckedValue330 G
;33G H
set33I L
{33M N#
isAddFriendCheckedValue33O f
=33g h
value33i n
;33n o
OnPropertyChanged	33p Å
(
33Å Ç
)
33Ç É
;
33É Ñ
if
33Ö á
(
33à â
value
33â é
)
33é è
{
33ê ë
SearchResults
33í ü
.
33ü †
Clear
33† •
(
33• ¶
)
33¶ ß
;
33ß ®
SearchQuery
33© ¥
=
33µ ∂
$str
33∑ π
;
33π ∫
}
33ª º
}
33Ω æ
}
33ø ¿
public44 
bool44 
IsRequestsChecked44 %
{44& '
get44( +
=>44, ."
isRequestsCheckedValue44/ E
;44E F
set44G J
{44K L"
isRequestsCheckedValue44M c
=44d e
value44f k
;44k l
OnPropertyChanged44m ~
(44~ 
)	44 Ä
;
44Ä Å
if
44Ç Ñ
(
44Ö Ü
value
44Ü ã
)
44ã å!
LoadRequestsCommand
44ç †
.
44† °
Execute
44° ®
(
44® ©
null
44© ≠
)
44≠ Æ
;
44Æ Ø
}
44∞ ±
}
44≤ ≥
public55 
bool55 
IsBusy55 
{55 
get55  
=>55! #
isBusyValue55$ /
;55/ 0
set551 4
{555 6
isBusyValue557 B
=55C D
value55E J
;55J K
OnPropertyChanged55L ]
(55] ^
)55^ _
;55_ `
(55a b
(55b c
RelayCommand55c o
)55o p#
LoadFriendsListCommand	55p Ü
)
55Ü á
.
55á à$
RaiseCanExecuteChanged
55à û
(
55û ü
)
55ü †
;
55† °
(
55¢ £
(
55£ §
RelayCommand
55§ ∞
)
55∞ ±!
LoadRequestsCommand
55± ƒ
)
55ƒ ≈
.
55≈ ∆$
RaiseCanExecuteChanged
55∆ ‹
(
55‹ ›
)
55› ﬁ
;
55ﬁ ﬂ
}
55Í Î
}
55Ï Ì
public77 
ICommand77 "
LoadFriendsListCommand77 .
{77/ 0
get771 4
;774 5
}776 7
public88 
ICommand88 
LoadRequestsCommand88 +
{88, -
get88. 1
;881 2
}883 4
public99 
ICommand99 
SearchCommand99 %
{99& '
get99( +
;99+ ,
}99- .
public:: 
ICommand:: 
SendRequestCommand:: *
{::+ ,
get::- 0
;::0 1
}::2 3
public;; 
ICommand;;  
AcceptRequestCommand;; ,
{;;- .
get;;/ 2
;;;2 3
};;4 5
public<< 
ICommand<< !
DeclineRequestCommand<< -
{<<. /
get<<0 3
;<<3 4
}<<5 6
public== 
ICommand== 
RemoveFriendCommand== +
{==, -
get==. 1
;==1 2
}==3 4
public>> 
ICommand>> 
BackCommand>> #
{>>$ %
get>>& )
;>>) *
}>>+ ,
private@@ 
readonly@@ 
Action@@ 
navigateBackAction@@  2
;@@2 3
privateAA 
stringAA 
currentUserUsernameAA *
=>AA+ -
SessionServiceAA. <
.AA< =
UsernameAA= E
;AAE F
publicCC 
SocialViewModelCC 
(CC 
ActionCC %
navigateBackCC& 2
)CC2 3
{DD 	
navigateBackActionEE 
=EE  
navigateBackEE! -
;EE- ."
LoadFriendsListCommandGG "
=GG# $
newGG% (
RelayCommandGG) 5
(GG5 6
asyncGG6 ;
(GG< =
paramGG= B
)GGB C
=>GGD F
awaitGGG L'
executeLoadFriendsListAsyncGGM h
(GGh i
)GGi j
,GGj k
(GGl m
paramGGm r
)GGr s
=>GGt v
!GGw x
IsBusyGGx ~
)GG~ 
;	GG Ä
LoadRequestsCommandHH 
=HH  !
newHH" %
RelayCommandHH& 2
(HH2 3
asyncHH3 8
(HH9 :
paramHH: ?
)HH? @
=>HHA C
awaitHHD I$
executeLoadRequestsAsyncHHJ b
(HHb c
)HHc d
,HHd e
(HHf g
paramHHg l
)HHl m
=>HHn p
!HHq r
IsBusyHHr x
)HHx y
;HHy z
SearchCommandII 
=II 
newII 
RelayCommandII  ,
(II, -
asyncII- 2
(II3 4
paramII4 9
)II9 :
=>II; =
awaitII> C
executeSearchAsyncIID V
(IIV W
)IIW X
,IIX Y
(IIZ [
paramII[ `
)II` a
=>IIb d
!IIe f
IsBusyIIf l
&&IIm o
!IIp q
stringIIq w
.IIw x
IsNullOrWhiteSpace	IIx ä
(
IIä ã
SearchQuery
IIã ñ
)
IIñ ó
)
IIó ò
;
IIò ô
SendRequestCommandJJ 
=JJ  
newJJ! $
RelayCommandJJ% 1
(JJ1 2
asyncJJ2 7
(JJ8 9
paramJJ9 >
)JJ> ?
=>JJ@ B
awaitJJC H#
executeSendRequestAsyncJJI `
(JJ` a
paramJJa f
asJJg i!
PlayerSearchResultDtoJJj 
)	JJ Ä
,
JJÄ Å
(
JJÇ É
param
JJÉ à
)
JJà â
=>
JJä å
!
JJç é
IsBusy
JJé î
&&
JJï ó
param
JJò ù
is
JJû †#
PlayerSearchResultDto
JJ° ∂
)
JJ∂ ∑
;
JJ∑ ∏ 
AcceptRequestCommandKK  
=KK! "
newKK# &
RelayCommandKK' 3
(KK3 4
asyncKK4 9
(KK: ;
paramKK; @
)KK@ A
=>KKB D
awaitKKE J&
executeRespondRequestAsyncKKK e
(KKe f
paramKKf k
asKKl n!
FriendRequestInfoDto	KKo É
,
KKÉ Ñ
true
KKÖ â
)
KKâ ä
,
KKä ã
(
KKå ç
param
KKç í
)
KKí ì
=>
KKî ñ
!
KKó ò
IsBusy
KKò û
&&
KKü °
param
KK¢ ß
is
KK® ™"
FriendRequestInfoDto
KK´ ø
)
KKø ¿
;
KK¿ ¡!
DeclineRequestCommandLL !
=LL" #
newLL$ '
RelayCommandLL( 4
(LL4 5
asyncLL5 :
(LL; <
paramLL< A
)LLA B
=>LLC E
awaitLLF K&
executeRespondRequestAsyncLLL f
(LLf g
paramLLg l
asLLm o!
FriendRequestInfoDto	LLp Ñ
,
LLÑ Ö
false
LLÜ ã
)
LLã å
,
LLå ç
(
LLé è
param
LLè î
)
LLî ï
=>
LLñ ò
!
LLô ö
IsBusy
LLö †
&&
LL° £
param
LL§ ©
is
LL™ ¨"
FriendRequestInfoDto
LL≠ ¡
)
LL¡ ¬
;
LL¬ √
RemoveFriendCommandMM 
=MM  !
newMM" %
RelayCommandMM& 2
(MM2 3
asyncMM3 8
(MM9 :
paramMM: ?
)MM? @
=>MMA C
awaitMMD I$
executeRemoveFriendAsyncMMJ b
(MMb c
paramMMc h
asMMi k
FriendDtoDisplayMMl |
)MM| }
,MM} ~
(	MM Ä
param
MMÄ Ö
)
MMÖ Ü
=>
MMá â
!
MMä ã
IsBusy
MMã ë
&&
MMí î
param
MMï ö
is
MMõ ù
FriendDtoDisplay
MMû Æ
)
MMÆ Ø
;
MMØ ∞
BackCommandNN 
=NN 
newNN 
RelayCommandNN *
(NN* +
(NN+ ,
paramNN, 1
)NN1 2
=>NN3 5
navigateBackActionNN6 H
?NNH I
.NNI J
InvokeNNJ P
(NNP Q
)NNQ R
)NNR S
;NNS T
connectAndSubscribePP 
(PP  
)PP  !
;PP! "
ifRR 
(RR  
IsFriendsListCheckedRR $
)RR$ %"
LoadFriendsListCommandRR& <
.RR< =
ExecuteRR= D
(RRD E
nullRRE I
)RRI J
;RRJ K
elseSS 
ifSS 
(SS 
IsRequestsCheckedSS &
)SS& '
LoadRequestsCommandSS( ;
.SS; <
ExecuteSS< C
(SSC D
nullSSD H
)SSH I
;SSI J
}TT 	
privateVV 
voidVV 
connectAndSubscribeVV (
(VV( )
)VV) *
{WW 	
ifXX 
(XX &
SocialServiceClientManagerXX *
.XX* +
instanceXX+ 3
.XX3 4
EnsureConnectedXX4 C
(XXC D
currentUserUsernameXXD W
)XXW X
)XXX Y
{YY 
ifZZ 
(ZZ 
callbackHandlerZZ #
!=ZZ$ &
nullZZ' +
)ZZ+ ,
{[[ 
callbackHandler\\ #
.\\# $!
FriendRequestReceived\\$ 9
-=\\: <'
handleFriendRequestReceived\\= X
;\\X Y
callbackHandler]] #
.]]# $"
FriendResponseReceived]]$ :
-=]]; =(
handleFriendResponseReceived]]> Z
;]]Z [
callbackHandler^^ #
.^^# $
FriendStatusChanged^^$ 7
-=^^8 :%
handleFriendStatusChanged^^; T
;^^T U
callbackHandler`` #
.``# $!
FriendRequestReceived``$ 9
+=``: <'
handleFriendRequestReceived``= X
;``X Y
callbackHandleraa #
.aa# $"
FriendResponseReceivedaa$ :
+=aa; =(
handleFriendResponseReceivedaa> Z
;aaZ [
callbackHandlerbb #
.bb# $
FriendStatusChangedbb$ 7
+=bb8 :%
handleFriendStatusChangedbb; T
;bbT U
Consolecc 
.cc 
	WriteLinecc %
(cc% &
$"cc& (
$strcc( [
{cc[ \
currentUserUsernamecc\ o
}cco p
$strccp q
"ccq r
)ccr s
;ccs t
}dd 
elseee 
{ee 
}ee, -
}ff 
elsegg 
{gg 
}gg( )
}hh 	
privatejj 
asyncjj 
Taskjj '
executeLoadFriendsListAsyncjj 6
(jj6 7
)jj7 8
{kk 	
ifll 
(ll 
!ll &
SocialServiceClientManagerll +
.ll+ ,
instancell, 4
.ll4 5
EnsureConnectedll5 D
(llD E
currentUserUsernamellE X
)llX Y
)llY Z
returnll[ a
;lla b
setBusymm 
(mm 
truemm 
)mm 
;mm 
FriendsListnn 
.nn 
Clearnn 
(nn 
)nn 
;nn  
tryoo 
{pp  
SocialManagerServiceqq $
.qq$ %
	FriendDtoqq% .
[qq. /
]qq/ 0
friendsqq1 8
=qq9 :
awaitqq; @
proxyqqA F
.qqF G
getFriendsListAsyncqqG Z
(qqZ [
currentUserUsernameqq[ n
)qqn o
;qqo p
ifrr 
(rr 
friendsrr 
!=rr 
nullrr #
)rr# $
{ss 
foreachtt 
(tt 
vartt  
	friendDtott! *
intt+ -
friendstt. 5
)tt5 6
{uu 
FriendsListvv #
.vv# $
Addvv$ '
(vv' (
newvv( +
FriendDtoDisplayvv, <
(vv< =
	friendDtovv= F
)vvF G
)vvG H
;vvH I
}ww 
}xx 
}yy 
catchzz 
(zz 
	Exceptionzz 
exzz 
)zz  
{zz! "
handleErrorzz# .
(zz. /
$strzz/ K
,zzK L
exzzM O
)zzO P
;zzP Q
}zzR S
finally{{ 
{{{ 
setBusy{{ 
({{ 
false{{ #
){{# $
;{{$ %
}{{& '
}|| 	
private~~ 
async~~ 
Task~~ $
executeLoadRequestsAsync~~ 3
(~~3 4
)~~4 5
{ 	
if
ÄÄ 
(
ÄÄ 
!
ÄÄ (
SocialServiceClientManager
ÄÄ +
.
ÄÄ+ ,
instance
ÄÄ, 4
.
ÄÄ4 5
EnsureConnected
ÄÄ5 D
(
ÄÄD E!
currentUserUsername
ÄÄE X
)
ÄÄX Y
)
ÄÄY Z
return
ÄÄ[ a
;
ÄÄa b
setBusy
ÅÅ 
(
ÅÅ 
true
ÅÅ 
)
ÅÅ 
;
ÅÅ 
ReceivedRequests
ÇÇ 
.
ÇÇ 
Clear
ÇÇ "
(
ÇÇ" #
)
ÇÇ# $
;
ÇÇ$ %
try
ÉÉ 
{
ÑÑ "
FriendRequestInfoDto
ÖÖ $
[
ÖÖ$ %
]
ÖÖ% &
requests
ÖÖ' /
=
ÖÖ0 1
await
ÖÖ2 7
proxy
ÖÖ8 =
.
ÖÖ= >$
getFriendRequestsAsync
ÖÖ> T
(
ÖÖT U!
currentUserUsername
ÖÖU h
)
ÖÖh i
;
ÖÖi j
if
ÜÜ 
(
ÜÜ 
requests
ÜÜ 
!=
ÜÜ 
null
ÜÜ  $
)
ÜÜ$ %
{
áá 
foreach
àà 
(
àà 
var
àà  
req
àà! $
in
àà% '
requests
àà( 0
)
àà0 1
ReceivedRequests
àà2 B
.
ààB C
Add
ààC F
(
ààF G
req
ààG J
)
ààJ K
;
ààK L
}
ââ 
}
ää 
catch
ãã 
(
ãã 
	Exception
ãã 
ex
ãã 
)
ãã  
{
ãã! "
handleError
ãã# .
(
ãã. /
$str
ãã/ N
,
ããN O
ex
ããP R
)
ããR S
;
ããS T
}
ããU V
finally
åå 
{
åå 
setBusy
åå 
(
åå 
false
åå #
)
åå# $
;
åå$ %
}
åå& '
}
çç 	
private
èè 
async
èè 
Task
èè  
executeSearchAsync
èè -
(
èè- .
)
èè. /
{
êê 	
if
ëë 
(
ëë 
!
ëë (
SocialServiceClientManager
ëë +
.
ëë+ ,
instance
ëë, 4
.
ëë4 5
EnsureConnected
ëë5 D
(
ëëD E!
currentUserUsername
ëëE X
)
ëëX Y
)
ëëY Z
return
ëë[ a
;
ëëa b
setBusy
íí 
(
íí 
true
íí 
)
íí 
;
íí 
SearchResults
ìì 
.
ìì 
Clear
ìì 
(
ìì  
)
ìì  !
;
ìì! "
try
îî 
{
ïï #
PlayerSearchResultDto
ññ %
[
ññ% &
]
ññ& '
results
ññ( /
=
ññ0 1
await
ññ2 7
proxy
ññ8 =
.
ññ= > 
searchPlayersAsync
ññ> P
(
ññP Q!
currentUserUsername
ññQ d
,
ññd e
SearchQuery
ññf q
)
ññq r
;
ññr s
if
óó 
(
óó 
results
óó 
!=
óó 
null
óó #
)
óó# $
{
òò 
foreach
ôô 
(
ôô 
var
ôô  
user
ôô! %
in
ôô& (
results
ôô) 0
)
ôô0 1
SearchResults
ôô2 ?
.
ôô? @
Add
ôô@ C
(
ôôC D
user
ôôD H
)
ôôH I
;
ôôI J
}
öö 
}
õõ 
catch
úú 
(
úú 
	Exception
úú 
ex
úú 
)
úú  
{
úú! "
handleError
úú# .
(
úú. /
$str
úú/ H
,
úúH I
ex
úúJ L
)
úúL M
;
úúM N
}
úúO P
finally
ùù 
{
ùù 
setBusy
ùù 
(
ùù 
false
ùù #
)
ùù# $
;
ùù$ %
}
ùù& '
}
ûû 	
private
†† 
async
†† 
Task
†† %
executeSendRequestAsync
†† 2
(
††2 3#
PlayerSearchResultDto
††3 H

targetUser
††I S
)
††S T
{
°° 	
if
¢¢ 
(
¢¢ 

targetUser
¢¢ 
==
¢¢ 
null
¢¢ "
||
¢¢# %
!
¢¢& '(
SocialServiceClientManager
¢¢' A
.
¢¢A B
instance
¢¢B J
.
¢¢J K
EnsureConnected
¢¢K Z
(
¢¢Z [!
currentUserUsername
¢¢[ n
)
¢¢n o
)
¢¢o p
return
¢¢q w
;
¢¢w x
setBusy
££ 
(
££ 
true
££ 
)
££ 
;
££ 
try
§§ 
{
••  
OperationResultDto
¶¶ "
result
¶¶# )
=
¶¶* +
await
¶¶, 1
proxy
¶¶2 7
.
¶¶7 8$
sendFriendRequestAsync
¶¶8 N
(
¶¶N O!
currentUserUsername
¶¶O b
,
¶¶b c

targetUser
¶¶d n
.
¶¶n o
username
¶¶o w
)
¶¶w x
;
¶¶x y

MessageBox
ßß 
.
ßß 
Show
ßß 
(
ßß  
result
ßß  &
.
ßß& '
message
ßß' .
,
ßß. /
result
ßß0 6
.
ßß6 7
success
ßß7 >
?
ßß? @
Lang
ßßA E
.
ßßE F!
InfoMsgTitleSuccess
ßßF Y
:
ßßZ [
$str
ßß\ c
,
ßßc d
MessageBoxButton
ßße u
.
ßßu v
OK
ßßv x
,
ßßx y
resultßßz Ä
.ßßÄ Å
successßßÅ à
?ßßâ ä
MessageBoxImageßßã ö
.ßßö õ
Informationßßõ ¶
:ßßß ®
MessageBoxImageßß© ∏
.ßß∏ π
Warningßßπ ¿
)ßß¿ ¡
;ßß¡ ¬
if
®® 
(
®® 
result
®® 
.
®® 
success
®® "
)
®®" #
{
©© 
SearchResults
™™ !
.
™™! "
Remove
™™" (
(
™™( )

targetUser
™™) 3
)
™™3 4
;
™™4 5
}
´´ 
}
¨¨ 
catch
≠≠ 
(
≠≠ 
	Exception
≠≠ 
ex
≠≠ 
)
≠≠  
{
≠≠! "
handleError
≠≠# .
(
≠≠. /
$str
≠≠/ M
,
≠≠M N
ex
≠≠O Q
)
≠≠Q R
;
≠≠R S
}
≠≠T U
finally
ÆÆ 
{
ÆÆ 
setBusy
ÆÆ 
(
ÆÆ 
false
ÆÆ #
)
ÆÆ# $
;
ÆÆ$ %
}
ÆÆ& '
}
ØØ 	
private
±± 
async
±± 
Task
±± (
executeRespondRequestAsync
±± 5
(
±±5 6"
FriendRequestInfoDto
±±6 J
request
±±K R
,
±±R S
bool
±±T X
accept
±±Y _
)
±±_ `
{
≤≤ 	
if
≥≥ 
(
≥≥ 
request
≥≥ 
==
≥≥ 
null
≥≥ 
||
≥≥  "
!
≥≥# $(
SocialServiceClientManager
≥≥$ >
.
≥≥> ?
instance
≥≥? G
.
≥≥G H
EnsureConnected
≥≥H W
(
≥≥W X!
currentUserUsername
≥≥X k
)
≥≥k l
)
≥≥l m
return
≥≥n t
;
≥≥t u
setBusy
¥¥ 
(
¥¥ 
true
¥¥ 
)
¥¥ 
;
¥¥ 
try
µµ 
{
∂∂  
OperationResultDto
∑∑ "
result
∑∑# )
=
∑∑* +
await
∑∑, 1
proxy
∑∑2 7
.
∑∑7 8)
respondToFriendRequestAsync
∑∑8 S
(
∑∑S T!
currentUserUsername
∑∑T g
,
∑∑g h
request
∑∑i p
.
∑∑p q 
requesterUsername∑∑q Ç
,∑∑Ç É
accept∑∑Ñ ä
)∑∑ä ã
;∑∑ã å

MessageBox
∏∏ 
.
∏∏ 
Show
∏∏ 
(
∏∏  
result
∏∏  &
.
∏∏& '
message
∏∏' .
,
∏∏. /
result
∏∏0 6
.
∏∏6 7
success
∏∏7 >
?
∏∏? @
Lang
∏∏A E
.
∏∏E F!
InfoMsgTitleSuccess
∏∏F Y
:
∏∏Z [
$str
∏∏\ c
,
∏∏c d
MessageBoxButton
∏∏e u
.
∏∏u v
OK
∏∏v x
,
∏∏x y
result∏∏z Ä
.∏∏Ä Å
success∏∏Å à
?∏∏â ä
MessageBoxImage∏∏ã ö
.∏∏ö õ
Information∏∏õ ¶
:∏∏ß ®
MessageBoxImage∏∏© ∏
.∏∏∏ π
Warning∏∏π ¿
)∏∏¿ ¡
;∏∏¡ ¬
if
ππ 
(
ππ 
result
ππ 
.
ππ 
success
ππ "
)
ππ" #
{
∫∫ 
ReceivedRequests
ªª $
.
ªª$ %
Remove
ªª% +
(
ªª+ ,
request
ªª, 3
)
ªª3 4
;
ªª4 5
if
ºº 
(
ºº 
accept
ºº 
&&
ºº !"
IsFriendsListChecked
ºº" 6
)
ºº6 7
{
ΩΩ 
await
ææ )
executeLoadFriendsListAsync
ææ 9
(
ææ9 :
)
ææ: ;
;
ææ; <
}
øø 
}
¿¿ 
}
¡¡ 
catch
¬¬ 
(
¬¬ 
	Exception
¬¬ 
ex
¬¬ 
)
¬¬  
{
¬¬! "
handleError
¬¬# .
(
¬¬. /
$str
¬¬/ S
,
¬¬S T
ex
¬¬U W
)
¬¬W X
;
¬¬X Y
}
¬¬Z [
finally
√√ 
{
√√ 
setBusy
√√ 
(
√√ 
false
√√ #
)
√√# $
;
√√$ %
}
√√& '
}
ƒƒ 	
private
∆∆ 
async
∆∆ 
Task
∆∆ &
executeRemoveFriendAsync
∆∆ 3
(
∆∆3 4
FriendDtoDisplay
∆∆4 D
friendToRemove
∆∆E S
)
∆∆S T
{
«« 	
if
»» 
(
»» 
friendToRemove
»» 
==
»» !
null
»»" &
||
»»' )
!
»»* +(
SocialServiceClientManager
»»+ E
.
»»E F
instance
»»F N
.
»»N O
EnsureConnected
»»O ^
(
»»^ _!
currentUserUsername
»»_ r
)
»»r s
)
»»s t
return
»»u {
;
»»{ |
var
   
confirmResult
   
=
   

MessageBox
    *
.
  * +
Show
  + /
(
  / 0
$"
  0 2
$str
  2 R
{
  R S
friendToRemove
  S a
.
  a b
Username
  b j
}
  j k
$str
  k l
"
  l m
,
  m n
$str
  o ~
,
  ~  
MessageBoxButton  Ä ê
.  ê ë
YesNo  ë ñ
,  ñ ó
MessageBoxImage  ò ß
.  ß ®
Question  ® ∞
)  ∞ ±
;  ± ≤
if
ÀÀ 
(
ÀÀ 
confirmResult
ÀÀ 
!=
ÀÀ  
MessageBoxResult
ÀÀ! 1
.
ÀÀ1 2
Yes
ÀÀ2 5
)
ÀÀ5 6
return
ÀÀ7 =
;
ÀÀ= >
setBusy
ÕÕ 
(
ÕÕ 
true
ÕÕ 
)
ÕÕ 
;
ÕÕ 
try
ŒŒ 
{
œœ  
OperationResultDto
–– "
result
––# )
=
––* +
await
––, 1
proxy
––2 7
.
––7 8
removeFriendAsync
––8 I
(
––I J!
currentUserUsername
––J ]
,
––] ^
friendToRemove
––_ m
.
––m n
Username
––n v
)
––v w
;
––w x

MessageBox
—— 
.
—— 
Show
—— 
(
——  
result
——  &
.
——& '
message
——' .
,
——. /
result
——0 6
.
——6 7
success
——7 >
?
——? @
Lang
——A E
.
——E F!
InfoMsgTitleSuccess
——F Y
:
——Z [
$str
——\ c
,
——c d
MessageBoxButton
——e u
.
——u v
OK
——v x
,
——x y
result——z Ä
.——Ä Å
success——Å à
?——â ä
MessageBoxImage——ã ö
.——ö õ
Information——õ ¶
:——ß ®
MessageBoxImage——© ∏
.——∏ π
Warning——π ¿
)——¿ ¡
;——¡ ¬
if
““ 
(
““ 
result
““ 
.
““ 
success
““ "
)
““" #
{
”” 
FriendsList
‘‘ 
.
‘‘  
Remove
‘‘  &
(
‘‘& '
friendToRemove
‘‘' 5
)
‘‘5 6
;
‘‘6 7
}
’’ 
}
÷÷ 
catch
◊◊ 
(
◊◊ 
	Exception
◊◊ 
ex
◊◊ 
)
◊◊  
{
◊◊! "
handleError
◊◊# .
(
◊◊. /
$str
◊◊/ F
,
◊◊F G
ex
◊◊H J
)
◊◊J K
;
◊◊K L
}
◊◊M N
finally
ÿÿ 
{
ÿÿ 
setBusy
ÿÿ 
(
ÿÿ 
false
ÿÿ #
)
ÿÿ# $
;
ÿÿ$ %
}
ÿÿ& '
}
ŸŸ 	
private
‹‹ 
void
‹‹ )
handleFriendRequestReceived
‹‹ 0
(
‹‹0 1
string
‹‹1 7
fromUsername
‹‹8 D
)
‹‹D E
{
›› 	
Application
ﬁﬁ 
.
ﬁﬁ 
Current
ﬁﬁ 
.
ﬁﬁ  

Dispatcher
ﬁﬁ  *
.
ﬁﬁ* +
Invoke
ﬁﬁ+ 1
(
ﬁﬁ1 2
async
ﬁﬁ2 7
(
ﬁﬁ8 9
)
ﬁﬁ9 :
=>
ﬁﬁ; =
{
ﬂﬂ 
if
‡‡ 
(
‡‡ 
IsRequestsChecked
‡‡ %
)
‡‡% &
{
‡‡' (
await
‡‡) .&
executeLoadRequestsAsync
‡‡/ G
(
‡‡G H
)
‡‡H I
;
‡‡I J
}
‡‡K L
}
·· 
)
·· 
;
·· 
}
‚‚ 	
private
‰‰ 
void
‰‰ *
handleFriendResponseReceived
‰‰ 1
(
‰‰1 2
string
‰‰2 8
fromUsername
‰‰9 E
,
‰‰E F
bool
‰‰G K
accepted
‰‰L T
)
‰‰T U
{
ÂÂ 	
Application
ÊÊ 
.
ÊÊ 
Current
ÊÊ 
.
ÊÊ  

Dispatcher
ÊÊ  *
.
ÊÊ* +
Invoke
ÊÊ+ 1
(
ÊÊ1 2
async
ÊÊ2 7
(
ÊÊ8 9
)
ÊÊ9 :
=>
ÊÊ; =
{
ÁÁ 
if
ËË 
(
ËË 
accepted
ËË 
&&
ËË "
IsFriendsListChecked
ËË  4
)
ËË4 5
{
ËË6 7
await
ËË8 =)
executeLoadFriendsListAsync
ËË> Y
(
ËËY Z
)
ËËZ [
;
ËË[ \
}
ËË] ^
}
ÈÈ 
)
ÈÈ 
;
ÈÈ 
}
ÍÍ 	
private
ÏÏ 
void
ÏÏ '
handleFriendStatusChanged
ÏÏ .
(
ÏÏ. /
string
ÏÏ/ 5
friendUsername
ÏÏ6 D
,
ÏÏD E
bool
ÏÏF J
isOnline
ÏÏK S
)
ÏÏS T
{
ÌÌ 	
Console
ÓÓ 
.
ÓÓ 
	WriteLine
ÓÓ 
(
ÓÓ 
$"
ÓÓ  
$str
ÓÓ  L
{
ÓÓL M
friendUsername
ÓÓM [
}
ÓÓ[ \
$str
ÓÓ\ h
{
ÓÓh i
isOnline
ÓÓi q
}
ÓÓq r
"
ÓÓr s
)
ÓÓs t
;
ÓÓt u
Application
ÔÔ 
.
ÔÔ 
Current
ÔÔ 
.
ÔÔ  

Dispatcher
ÔÔ  *
.
ÔÔ* +
Invoke
ÔÔ+ 1
(
ÔÔ1 2
(
ÔÔ2 3
)
ÔÔ3 4
=>
ÔÔ5 7
{
 
var
ÒÒ 
friend
ÒÒ 
=
ÒÒ 
FriendsList
ÒÒ (
.
ÒÒ( )
FirstOrDefault
ÒÒ) 7
(
ÒÒ7 8
f
ÒÒ8 9
=>
ÒÒ: <
f
ÒÒ= >
.
ÒÒ> ?
Username
ÒÒ? G
.
ÒÒG H
Equals
ÒÒH N
(
ÒÒN O
friendUsername
ÒÒO ]
,
ÒÒ] ^
StringComparison
ÒÒ_ o
.
ÒÒo p 
OrdinalIgnoreCaseÒÒp Å
)ÒÒÅ Ç
)ÒÒÇ É
;ÒÒÉ Ñ
if
ÚÚ 
(
ÚÚ 
friend
ÚÚ 
!=
ÚÚ 
null
ÚÚ "
)
ÚÚ" #
{
ÛÛ 
Console
ÙÙ 
.
ÙÙ 
	WriteLine
ÙÙ %
(
ÙÙ% &
$"
ÙÙ& (
$str
ÙÙ( F
{
ÙÙF G
friendUsername
ÙÙG U
}
ÙÙU V
$str
ÙÙV u
{
ÙÙu v
isOnline
ÙÙv ~
}
ÙÙ~ 
$strÙÙ Ä
"ÙÙÄ Å
)ÙÙÅ Ç
;ÙÙÇ É
friend
ıı 
.
ıı 
IsOnline
ıı #
=
ıı$ %
isOnline
ıı& .
;
ıı. /
}
ˆˆ 
else
˜˜ 
{
˜˜ 
Console
˜˜ 
.
˜˜ 
	WriteLine
˜˜ (
(
˜˜( )
$"
˜˜) +
$str
˜˜+ C
{
˜˜C D
friendUsername
˜˜D R
}
˜˜R S
$str
˜˜S u
"
˜˜u v
)
˜˜v w
;
˜˜w x
}
˜˜y z
}
¯¯ 
)
¯¯ 
;
¯¯ 
}
˘˘ 	
private
˚˚ 
void
˚˚ 
setBusy
˚˚ 
(
˚˚ 
bool
˚˚ !
busy
˚˚" &
)
˚˚& '
{
˚˚( )
IsBusy
˚˚* 0
=
˚˚1 2
busy
˚˚3 7
;
˚˚7 8
Application
˚˚9 D
.
˚˚D E
Current
˚˚E L
.
˚˚L M

Dispatcher
˚˚M W
?
˚˚W X
.
˚˚X Y
Invoke
˚˚Y _
(
˚˚_ `
(
˚˚` a
)
˚˚a b
=>
˚˚c e
CommandManager
˚˚f t
.
˚˚t u)
InvalidateRequerySuggested˚˚u è
(˚˚è ê
)˚˚ê ë
)˚˚ë í
;˚˚í ì
}˚˚î ï
private
¸¸ 
void
¸¸ 
handleError
¸¸  
(
¸¸  !
string
¸¸! '
message
¸¸( /
,
¸¸/ 0
	Exception
¸¸1 :
ex
¸¸; =
)
¸¸= >
{
¸¸? @
Console
¸¸A H
.
¸¸H I
	WriteLine
¸¸I R
(
¸¸R S
$"
¸¸S U
$str
¸¸U Y
{
¸¸Y Z
message
¸¸Z a
}
¸¸a b
$str
¸¸b d
{
¸¸d e
ex
¸¸e g
}
¸¸g h
"
¸¸h i
)
¸¸i j
;
¸¸j k

MessageBox
¸¸l v
.
¸¸v w
Show
¸¸w {
(
¸¸{ |
$"
¸¸| ~
{
¸¸~ 
message¸¸ Ü
}¸¸Ü á
$str¸¸á â
{¸¸â ä
ex¸¸ä å
.¸¸å ç
Message¸¸ç î
}¸¸î ï
"¸¸ï ñ
,¸¸ñ ó
$str¸¸ò ü
,¸¸ü † 
MessageBoxButton¸¸° ±
.¸¸± ≤
OK¸¸≤ ¥
,¸¸¥ µ
MessageBoxImage¸¸∂ ≈
.¸¸≈ ∆
Error¸¸∆ À
)¸¸À Ã
;¸¸Ã Õ
}¸¸Œ œ
public
˛˛ 
void
˛˛ 
cleanup
˛˛ 
(
˛˛ 
)
˛˛ 
{
ˇˇ 	
if
ÄÄ 
(
ÄÄ 
callbackHandler
ÄÄ 
!=
ÄÄ  "
null
ÄÄ# '
)
ÄÄ' (
{
ÅÅ 
callbackHandler
ÇÇ 
.
ÇÇ  #
FriendRequestReceived
ÇÇ  5
-=
ÇÇ6 8)
handleFriendRequestReceived
ÇÇ9 T
;
ÇÇT U
callbackHandler
ÉÉ 
.
ÉÉ  $
FriendResponseReceived
ÉÉ  6
-=
ÉÉ7 9*
handleFriendResponseReceived
ÉÉ: V
;
ÉÉV W
callbackHandler
ÑÑ 
.
ÑÑ  !
FriendStatusChanged
ÑÑ  3
-=
ÑÑ4 6'
handleFriendStatusChanged
ÑÑ7 P
;
ÑÑP Q
Console
ÖÖ 
.
ÖÖ 
	WriteLine
ÖÖ !
(
ÖÖ! "
$"
ÖÖ" $
$str
ÖÖ$ [
{
ÖÖ[ \!
currentUserUsername
ÖÖ\ o
}
ÖÖo p
$str
ÖÖp q
"
ÖÖq r
)
ÖÖr s
;
ÖÖs t
}
ÜÜ 
}
áá 	
}
ââ 
}ää ‚Y
ìC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\ViewModel\Main\SettingsViewModel.cs
	namespace 	
MindWeaveClient
 
. 
	ViewModel #
.# $
Main$ (
{		 
public

 

class

 
LanguageOption

 
{ 
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Code 
{ 
get  
;  !
set" %
;% &
}' (
public 
override 
string 
ToString '
(' (
)( )
{ 	
return 
Name 
; 
} 	
} 
public 

class 
SettingsViewModel "
:# $
BaseViewModel% 2
{ 
private 
double 
musicVolumeValue '
;' (
private 
double #
soundEffectsVolumeValue .
;. /
private 
LanguageOption !
selectedLanguageValue 4
;4 5
private 
List 
< 
LanguageOption #
># $#
availableLanguagesValue% <
;< =
public 
double 
MusicVolume !
{ 	
get 
=> 
musicVolumeValue #
;# $
set 
{   
musicVolumeValue!!  
=!!! "
value!!# (
;!!( )
OnPropertyChanged"" !
(""! "
)""" #
;""# $
AudioManager## 
.## 
setMusicVolume## +
(##+ ,
value##, 1
/##2 3
$num##4 9
)##9 :
;##: ;
}$$ 
}%% 	
public'' 
double'' 
SoundEffectsVolume'' (
{(( 	
get)) 
=>)) #
soundEffectsVolumeValue)) *
;))* +
set** 
{++ #
soundEffectsVolumeValue,, '
=,,( )
value,,* /
;,,/ 0
OnPropertyChanged-- !
(--! "
)--" #
;--# $
AudioManager.. 
... !
setSoundEffectsVolume.. 2
(..2 3
value..3 8
/..9 :
$num..; @
)..@ A
;..A B
}// 
}00 	
public22 
LanguageOption22 
SelectedLanguage22 .
{33 	
get44 
=>44 !
selectedLanguageValue44 (
;44( )
set55 
{55 !
selectedLanguageValue55 '
=55( )
value55* /
;55/ 0
OnPropertyChanged551 B
(55B C
)55C D
;55D E
}55F G
}66 	
public88 
List88 
<88 
LanguageOption88 "
>88" #
AvailableLanguages88$ 6
{99 	
get:: 
=>:: #
availableLanguagesValue:: *
;::* +
private;; 
set;; 
{;; #
availableLanguagesValue;; 1
=;;2 3
value;;4 9
;;;9 :
OnPropertyChanged;;; L
(;;L M
);;M N
;;;N O
};;P Q
}<< 	
public>> 
ICommand>> 
SaveCommand>> #
{>>$ %
get>>& )
;>>) *
}>>+ ,
public?? 
ICommand?? 
CancelCommand?? %
{??& '
get??( +
;??+ ,
}??- .
public@@ 
ICommand@@ 
ShowCreditsCommand@@ *
{@@+ ,
get@@- 0
;@@0 1
}@@2 3
privateBB 
WindowBB 
currentWindowBB $
;BB$ %
publicDD 
SettingsViewModelDD  
(DD  !
WindowDD! '
windowDD( .
)DD. /
{EE 	
currentWindowFF 
=FF 
windowFF "
;FF" #
loadSettingsGG 
(GG 
)GG 
;GG 
initializeLanguagesHH 
(HH  
)HH  !
;HH! "
SaveCommandJJ 
=JJ 
newJJ 
RelayCommandJJ *
(JJ* +
pJJ+ ,
=>JJ- /
executeSaveJJ0 ;
(JJ; <
)JJ< =
)JJ= >
;JJ> ?
CancelCommandKK 
=KK 
newKK 
RelayCommandKK  ,
(KK, -
pKK- .
=>KK/ 1
executeCancelKK2 ?
(KK? @
)KK@ A
)KKA B
;KKB C
ShowCreditsCommandLL 
=LL  
newLL! $
RelayCommandLL% 1
(LL1 2
pLL2 3
=>LL4 6
executeShowCreditsLL7 I
(LLI J
)LLJ K
)LLK L
;LLL M
}MM 	
privateOO 
voidOO 
initializeLanguagesOO (
(OO( )
)OO) *
{PP 	
AvailableLanguagesQQ 
=QQ  
newQQ! $
ListQQ% )
<QQ) *
LanguageOptionQQ* 8
>QQ8 9
{RR 
newSS 
LanguageOptionSS "
{SS# $
NameSS% )
=SS* +
LangSS, 0
.SS0 1
SettingsOptEnglishSS1 C
,SSC D
CodeSSE I
=SSJ K
$strSSL S
}SST U
,SSU V
newTT 
LanguageOptionTT "
{TT# $
NameTT% )
=TT* +
LangTT, 0
.TT0 1
SettingsOptSpanishTT1 C
,TTC D
CodeTTE I
=TTJ K
$strTTL S
}TTT U
}UU 
;UU 
stringWW 
currentLangCodeWW "
=WW# $

PropertiesWW% /
.WW/ 0
SettingsWW0 8
.WW8 9
DefaultWW9 @
.WW@ A
languageCodeWWA M
;WWM N
SelectedLanguageXX 
=XX 
AvailableLanguagesXX 1
.XX1 2
FirstOrDefaultXX2 @
(XX@ A
langXXA E
=>XXF H
langXXI M
.XXM N
CodeXXN R
==XXS U
currentLangCodeXXV e
)XXe f
??XXg i
AvailableLanguagesXXj |
.XX| }
First	XX} Ç
(
XXÇ É
)
XXÉ Ñ
;
XXÑ Ö
}YY 	
private[[ 
void[[ 
loadSettings[[ !
([[! "
)[[" #
{\\ 	
MusicVolume]] 
=]] 

Properties]] $
.]]$ %
Settings]]% -
.]]- .
Default]]. 5
.]]5 6
MusicVolumeSetting]]6 H
;]]H I
SoundEffectsVolume^^ 
=^^  

Properties^^! +
.^^+ ,
Settings^^, 4
.^^4 5
Default^^5 <
.^^< =%
SoundEffectsVolumeSetting^^= V
;^^V W
AudioManager`` 
.`` 
setMusicVolume`` '
(``' (
MusicVolume``( 3
/``4 5
$num``6 ;
)``; <
;``< =
AudioManageraa 
.aa !
setSoundEffectsVolumeaa .
(aa. /
SoundEffectsVolumeaa/ A
/aaB C
$numaaD I
)aaI J
;aaJ K
}bb 	
privatedd 
voiddd 
executeSavedd  
(dd  !
)dd! "
{ee 	

Propertiesff 
.ff 
Settingsff 
.ff  
Defaultff  '
.ff' (
MusicVolumeSettingff( :
=ff; <
MusicVolumeff= H
;ffH I

Propertiesgg 
.gg 
Settingsgg 
.gg  
Defaultgg  '
.gg' (%
SoundEffectsVolumeSettinggg( A
=ggB C
SoundEffectsVolumeggD V
;ggV W
stringhh  
previousLanguageCodehh '
=hh( )

Propertieshh* 4
.hh4 5
Settingshh5 =
.hh= >
Defaulthh> E
.hhE F
languageCodehhF R
;hhR S

Propertiesii 
.ii 
Settingsii 
.ii  
Defaultii  '
.ii' (
languageCodeii( 4
=ii5 6
SelectedLanguageii7 G
.iiG H
CodeiiH L
;iiL M

Propertiesjj 
.jj 
Settingsjj 
.jj  
Defaultjj  '
.jj' (
Savejj( ,
(jj, -
)jj- .
;jj. /
AudioManagerkk 
.kk 
setMusicVolumekk '
(kk' (
MusicVolumekk( 3
/kk4 5
$numkk6 ;
)kk; <
;kk< =
AudioManagerll 
.ll !
setSoundEffectsVolumell .
(ll. /
SoundEffectsVolumell/ A
/llB C
$numllD I
)llI J
;llJ K
ifnn 
(nn  
previousLanguageCodenn $
!=nn% '
SelectedLanguagenn( 8
.nn8 9
Codenn9 =
)nn= >
{oo 
applyLanguageChangepp #
(pp# $
)pp$ %
;pp% &
}qq 
ifrr 
(rr 
currentWindowrr 
!=rr  
nullrr! %
)rr% &
{ss 
currentWindowtt 
.tt 
DialogResulttt *
=tt+ ,
truett- 1
;tt1 2
currentWindowuu 
.uu 
Closeuu #
(uu# $
)uu$ %
;uu% &
}vv 
}ww 	
privateyy 
voidyy 
applyLanguageChangeyy (
(yy( )
)yy) *
{zz 	
MessageBoxResult{{ 
result{{ #
={{$ %

MessageBox{{& 0
.{{0 1
Show{{1 5
({{5 6
$str|| c
,||c d
$str}} "
,}}" #
MessageBoxButton~~  
.~~  !
YesNo~~! &
,~~& '
MessageBoxImage 
.  
Information  +
)+ ,
;, -
if
ÅÅ 
(
ÅÅ 
result
ÅÅ 
==
ÅÅ 
MessageBoxResult
ÅÅ *
.
ÅÅ* +
Yes
ÅÅ+ .
)
ÅÅ. /
{
ÇÇ 
System
ÉÉ 
.
ÉÉ 
Diagnostics
ÉÉ "
.
ÉÉ" #
Process
ÉÉ# *
.
ÉÉ* +
Start
ÉÉ+ 0
(
ÉÉ0 1
Application
ÉÉ1 <
.
ÉÉ< =
ResourceAssembly
ÉÉ= M
.
ÉÉM N
Location
ÉÉN V
)
ÉÉV W
;
ÉÉW X
Application
ÑÑ 
.
ÑÑ 
Current
ÑÑ #
.
ÑÑ# $
Shutdown
ÑÑ$ ,
(
ÑÑ, -
)
ÑÑ- .
;
ÑÑ. /
}
ÖÖ 
}
ÜÜ 	
private
ââ 
void
ââ 
executeCancel
ââ "
(
ââ" #
)
ââ# $
{
ää 	
if
ãã 
(
ãã 
currentWindow
ãã 
!=
ãã  
null
ãã! %
)
ãã% &
{
åå 
currentWindow
çç 
.
çç 
DialogResult
çç *
=
çç+ ,
false
çç- 2
;
çç2 3
currentWindow
éé 
.
éé 
Close
éé #
(
éé# $
)
éé$ %
;
éé% &
}
èè 
}
êê 	
private
íí 
void
íí  
executeShowCredits
íí '
(
íí' (
)
íí( )
{
ìì 	

MessageBox
îî 
.
îî 
Show
îî 
(
îî 
$str
îî {
,
îî{ |
$strîî} Ü
)îîÜ á
;îîá à
}
ïï 	
}
ññ 
}óó Ø–
öC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\ViewModel\Main\SelectionPuzzleViewModel.cs
	namespace 	
MindWeaveClient
 
. 
	ViewModel #
.# $
Main$ (
{ 
public 

class 
PuzzleDisplayInfo "
:# $
BaseViewModel% 2
{ 
private 
bool 
isSelectedValue $
;$ %
private 
int 
puzzleIdValue !
;! "
private 
string 
	nameValue  
;  !
private 
string 
imagePathValue %
;% &
public 
int 
PuzzleId 
{ 
get !
=>" $
puzzleIdValue% 2
;2 3
set4 7
{8 9
puzzleIdValue: G
=H I
valueJ O
;O P
OnPropertyChangedQ b
(b c
)c d
;d e
}f g
}h i
public 
string 
Name 
{ 
get  
=>! #
	nameValue$ -
;- .
set/ 2
{3 4
	nameValue5 >
=? @
valueA F
;F G
OnPropertyChangedH Y
(Y Z
)Z [
;[ \
}] ^
}_ `
public 
string 
	ImagePath 
{  !
get" %
=>& (
imagePathValue) 7
;7 8
set9 <
{= >
imagePathValue? M
=N O
valueP U
;U V
OnPropertyChangedW h
(h i
)i j
;j k
}l m
}n o
public 
bool 

IsSelected 
{ 	
get 
=> 
isSelectedValue "
;" #
set 
{ 
isSelectedValue !
=" #
value$ )
;) *
OnPropertyChanged+ <
(< =
)= >
;> ?
}@ A
}   	
}!! 
public## 

class## $
SelectionPuzzleViewModel## )
:##* +
BaseViewModel##, 9
{$$ 
private%% 
readonly%% 
Action%% 
<%%  
Page%%  $
>%%$ %

navigateTo%%& 0
;%%0 1
private&& 
readonly&& 
Action&& 
navigateBack&&  ,
;&&, -
private'' 
PuzzleManagerClient'' #
puzzleClient''$ 0
;''0 1
private(( $
MatchmakingManagerClient(( (
matchmakingClient(() :
=>((; =+
MatchmakingServiceClientManager((> ]
.((] ^
instance((^ f
.((f g
proxy((g l
;((l m
private**  
ObservableCollection** $
<**$ %
PuzzleDisplayInfo**% 6
>**6 7!
availablePuzzlesValue**8 M
=**N O
new**P S 
ObservableCollection**T h
<**h i
PuzzleDisplayInfo**i z
>**z {
(**{ |
)**| }
;**} ~
private++ 
PuzzleDisplayInfo++ !
selectedPuzzleValue++" 5
;++5 6
private,, 
bool,, 
isBusyValue,,  
;,,  !
public..  
ObservableCollection.. #
<..# $
PuzzleDisplayInfo..$ 5
>..5 6
AvailablePuzzles..7 G
{// 	
get00 
=>00 !
availablePuzzlesValue00 (
;00( )
set11 
{11 !
availablePuzzlesValue11 '
=11( )
value11* /
;11/ 0
OnPropertyChanged111 B
(11B C
)11C D
;11D E
}11F G
}22 	
public44 
PuzzleDisplayInfo44  
SelectedPuzzle44! /
{55 	
get66 
=>66 
selectedPuzzleValue66 &
;66& '
set77 
{77 
selectedPuzzleValue77 %
=77& '
value77( -
;77- .
OnPropertyChanged77/ @
(77@ A
)77A B
;77B C
(77D E
(77E F
RelayCommand77F R
)77R S(
ConfirmAndCreateLobbyCommand77S o
)77o p
.77p q#
RaiseCanExecuteChanged	77q á
(
77á à
)
77à â
;
77â ä
}
77ã å
}88 	
public:: 
bool:: 
IsBusy:: 
{;; 	
get<< 
=><< 
isBusyValue<< 
;<< 
private== 
set== 
{== 
setBusy== !
(==! "
value==" '
)==' (
;==( )
}==* +
}>> 	
public@@ 
ICommand@@ 
LoadPuzzlesCommand@@ *
{@@+ ,
get@@- 0
;@@0 1
}@@2 3
publicAA 
ICommandAA 
UploadImageCommandAA *
{AA+ ,
getAA- 0
;AA0 1
}AA2 3
publicBB 
ICommandBB (
ConfirmAndCreateLobbyCommandBB 4
{BB5 6
getBB7 :
;BB: ;
}BB< =
publicCC 
ICommandCC 
CancelCommandCC %
{CC& '
getCC( +
;CC+ ,
}CC- .
publicDD 
ICommandDD 
SelectPuzzleCommandDD +
{DD, -
getDD. 1
;DD1 2
}DD3 4
publicFF $
SelectionPuzzleViewModelFF '
(FF' (
ActionFF( .
<FF. /
PageFF/ 3
>FF3 4
navigateToActionFF5 E
,FFE F
ActionFFG M
navigateBackActionFFN `
)FF` a
{GG 	

navigateToHH 
=HH 
navigateToActionHH )
;HH) *
navigateBackII 
=II 
navigateBackActionII -
;II- .
tryJJ 
{KK 
puzzleClientLL 
=LL 
newLL "
PuzzleManagerClientLL# 6
(LL6 7
)LL7 8
;LL8 9
}MM 
catchNN 
(NN 
	ExceptionNN 
exNN 
)NN  
{OO 

MessageBoxPP 
.PP 
ShowPP 
(PP  
$"PP  "
$strPP" J
{PPJ K
exPPK M
.PPM N
MessagePPN U
}PPU V
"PPV W
,PPW X
LangPPY ]
.PP] ^

ErrorTitlePP^ h
,PPh i
MessageBoxButtonPPj z
.PPz {
OKPP{ }
,PP} ~
MessageBoxImage	PP é
.
PPé è
Stop
PPè ì
)
PPì î
;
PPî ï
puzzleClientQQ 
=QQ 
nullQQ #
;QQ# $
}RR 
LoadPuzzlesCommandTT 
=TT  
newTT! $
RelayCommandTT% 1
(TT1 2
asyncTT2 7
pTT8 9
=>TT: <
awaitTT= B#
executeLoadPuzzlesAsyncTTC Z
(TTZ [
)TT[ \
,TT\ ]
pTT^ _
=>TT` b
!TTc d
IsBusyTTd j
&&TTk m
puzzleClientTTn z
!=TT{ }
null	TT~ Ç
)
TTÇ É
;
TTÉ Ñ
UploadImageCommandUU 
=UU  
newUU! $
RelayCommandUU% 1
(UU1 2
asyncUU2 7
pUU8 9
=>UU: <
awaitUU= B#
executeUploadImageAsyncUUC Z
(UUZ [
)UU[ \
,UU\ ]
pUU^ _
=>UU` b
!UUc d
IsBusyUUd j
&&UUk m
puzzleClientUUn z
!=UU{ }
null	UU~ Ç
)
UUÇ É
;
UUÉ Ñ(
ConfirmAndCreateLobbyCommandVV (
=VV) *
newVV+ .
RelayCommandVV/ ;
(VV; <
asyncVV< A
pVVB C
=>VVD F
awaitVVG L-
!executeConfirmAndCreateLobbyAsyncVVM n
(VVn o
)VVo p
,VVp q
pVVr s
=>VVt v%
canConfirmAndCreateLobby	VVw è
(
VVè ê
)
VVê ë
)
VVë í
;
VVí ì
CancelCommandWW 
=WW 
newWW 
RelayCommandWW  ,
(WW, -
pWW- .
=>WW/ 1
navigateBackWW2 >
?WW> ?
.WW? @
InvokeWW@ F
(WWF G
)WWG H
,WWH I
pWWJ K
=>WWL N
!WWO P
IsBusyWWP V
)WWV W
;WWW X
SelectPuzzleCommandXX 
=XX  !
newXX" %
RelayCommandXX& 2
(XX2 3
executeSelectPuzzleXX3 F
,XXF G
pXXH I
=>XXJ L
pXXM N
isXXO Q
PuzzleDisplayInfoXXR c
)XXc d
;XXd e
ifZZ 
(ZZ 
puzzleClientZZ 
!=ZZ 
nullZZ  $
)ZZ$ %
{[[ 
LoadPuzzlesCommand\\ "
.\\" #
Execute\\# *
(\\* +
null\\+ /
)\\/ 0
;\\0 1
}]] 
}^^ 	
private`` 
bool`` $
canConfirmAndCreateLobby`` -
(``- .
)``. /
{aa 	
returnbb 
SelectedPuzzlebb !
!=bb" $
nullbb% )
&&bb* ,
!bb- .
IsBusybb. 4
&&bb5 7
puzzleClientbb8 D
!=bbE G
nullbbH L
;bbL M
}cc 	
privateee 
voidee 
executeSelectPuzzleee (
(ee( )
objectee) /
	parameteree0 9
)ee9 :
{ff 	
ifgg 
(gg 
	parametergg 
isgg 
PuzzleDisplayInfogg .

puzzleInfogg/ 9
)gg9 :
{hh 
ifii 
(ii 
SelectedPuzzleii "
!=ii# %
nullii& *
&&ii+ -
SelectedPuzzleii. <
!=ii= ?

puzzleInfoii@ J
)iiJ K
{jj 
SelectedPuzzlekk "
.kk" #

IsSelectedkk# -
=kk. /
falsekk0 5
;kk5 6
}ll 
ifmm 
(mm 
SelectedPuzzlemm "
!=mm# %

puzzleInfomm& 0
)mm0 1
{nn 
SelectedPuzzleoo "
=oo# $

puzzleInfooo% /
;oo/ 0
ifpp 
(pp 
!pp 
SelectedPuzzlepp '
.pp' (

IsSelectedpp( 2
)pp2 3
{qq 
SelectedPuzzlerr &
.rr& '

IsSelectedrr' 1
=rr2 3
truerr4 8
;rr8 9
}ss 
OnPropertyChangedtt %
(tt% &
nameoftt& ,
(tt, -
SelectedPuzzlett- ;
)tt; <
)tt< =
;tt= >
(uu 
(uu 
RelayCommanduu "
)uu" #(
ConfirmAndCreateLobbyCommanduu# ?
)uu? @
.uu@ A"
RaiseCanExecuteChangeduuA W
(uuW X
)uuX Y
;uuY Z
}vv 
elseww 
ifww 
(ww 
!ww 
SelectedPuzzleww (
.ww( )

IsSelectedww) 3
)ww3 4
{xx 
SelectedPuzzleyy "
.yy" #

IsSelectedyy# -
=yy. /
trueyy0 4
;yy4 5
}zz 
}{{ 
}|| 	
private 
async 
Task #
executeLoadPuzzlesAsync 2
(2 3
)3 4
{
ÄÄ 	
if
ÅÅ 
(
ÅÅ 
puzzleClient
ÅÅ 
==
ÅÅ 
null
ÅÅ  $
)
ÅÅ$ %
return
ÅÅ& ,
;
ÅÅ, -
IsBusy
ÉÉ 
=
ÉÉ 
true
ÉÉ 
;
ÉÉ 
AvailablePuzzles
ÑÑ 
.
ÑÑ 
Clear
ÑÑ "
(
ÑÑ" #
)
ÑÑ# $
;
ÑÑ$ %
SelectedPuzzle
ÖÖ 
=
ÖÖ 
null
ÖÖ !
;
ÖÖ! "
PuzzleInfoDto
ÜÜ 
[
ÜÜ 
]
ÜÜ 
puzzlesFromServer
ÜÜ -
;
ÜÜ- .
try
àà 
{
ââ 
puzzlesFromServer
ää !
=
ää" #
await
ää$ )
puzzleClient
ää* 6
.
ää6 7&
getAvailablePuzzlesAsync
ää7 O
(
ääO P
)
ääP Q
;
ääQ R
if
çç 
(
çç 
puzzlesFromServer
çç %
!=
çç& (
null
çç) -
)
çç- .
{
éé 
Console
èè 
.
èè 
	WriteLine
èè %
(
èè% &
$"
èè& (
$str
èè( 1
{
èè1 2
puzzlesFromServer
èè2 C
.
èèC D
Length
èèD J
}
èèJ K
$str
èèK `
"
èè` a
)
èèa b
;
èèb c
foreach
êê 
(
êê 
var
êê  
pzlDto
êê! '
in
êê( *
puzzlesFromServer
êê+ <
)
êê< =
{
ëë 
string
íí 
clientImagePath
íí .
=
íí/ 0
pzlDto
íí1 7
.
íí7 8
	imagePath
íí8 A
;
ííA B
if
ìì 
(
ìì 
!
ìì 
string
ìì #
.
ìì# $
IsNullOrEmpty
ìì$ 1
(
ìì1 2
clientImagePath
ìì2 A
)
ììA B
&&
ììC E
!
ììF G
clientImagePath
ììG V
.
ììV W

StartsWith
ììW a
(
ììa b
$str
ììb e
)
ììe f
)
ììf g
{
îî 
clientImagePath
ïï +
=
ïï, -
$"
ïï. 0
$str
ïï0 J
{
ïïJ K
pzlDto
ïïK Q
.
ïïQ R
	imagePath
ïïR [
}
ïï[ \
"
ïï\ ]
;
ïï] ^
}
ññ 
AvailablePuzzles
òò (
.
òò( )
Add
òò) ,
(
òò, -
new
òò- 0
PuzzleDisplayInfo
òò1 B
{
ôô 
PuzzleId
öö $
=
öö% &
pzlDto
öö' -
.
öö- .
puzzleId
öö. 6
,
öö6 7
Name
õõ  
=
õõ! "
pzlDto
õõ# )
.
õõ) *
name
õõ* .
,
õõ. /
	ImagePath
úú %
=
úú& '
clientImagePath
úú( 7
}
ùù 
)
ùù 
;
ùù 
}
ûû 
OnPropertyChanged
üü %
(
üü% &
nameof
üü& ,
(
üü, -
AvailablePuzzles
üü- =
)
üü= >
)
üü> ?
;
üü? @
}
†† 
else
°° 
{
¢¢ 
Console
££ 
.
££ 
	WriteLine
££ %
(
££% &
$str
££& N
)
££N O
;
££O P
}
§§ 
}
•• 
catch
¶¶ 
(
¶¶ 
	Exception
¶¶ 
ex
¶¶ 
)
¶¶  
{
ßß 

MessageBox
®® 
.
®® 
Show
®® 
(
®®  
$"
®®  "
{
®®" #
Lang
®®# '
.
®®' (!
ErrorLoadingPuzzles
®®( ;
}
®®; <
$str
®®< >
{
®®> ?
ex
®®? A
.
®®A B
Message
®®B I
}
®®I J
"
®®J K
,
®®K L
Lang
®®M Q
.
®®Q R

ErrorTitle
®®R \
,
®®\ ]
MessageBoxButton
®®^ n
.
®®n o
OK
®®o q
,
®®q r
MessageBoxImage®®s Ç
.®®Ç É
Error®®É à
)®®à â
;®®â ä
if
©© 
(
©© 
ex
©© 
.
©© 
InnerException
©© %
!=
©©& (
null
©©) -
)
©©- .
{
™™ 

MessageBox
´´ 
.
´´ 
Show
´´ #
(
´´# $
$"
´´$ &
$str
´´& 7
{
´´7 8
ex
´´8 :
.
´´: ;
InnerException
´´; I
.
´´I J
Message
´´J Q
}
´´Q R
"
´´R S
,
´´S T
Lang
´´U Y
.
´´Y Z

ErrorTitle
´´Z d
,
´´d e
MessageBoxButton
´´f v
.
´´v w
OK
´´w y
,
´´y z
MessageBoxImage´´{ ä
.´´ä ã
Error´´ã ê
)´´ê ë
;´´ë í
}
¨¨ 
}
≠≠ 
finally
ÆÆ 
{
ØØ 
IsBusy
∞∞ 
=
∞∞ 
false
∞∞ 
;
∞∞ 
}
±± 
}
≤≤ 	
private
¥¥ 
async
¥¥ 
Task
¥¥ %
executeUploadImageAsync
¥¥ 2
(
¥¥2 3
)
¥¥3 4
{
µµ 	
if
∂∂ 
(
∂∂ 
puzzleClient
∂∂ 
==
∂∂ 
null
∂∂  $
)
∂∂$ %
return
∂∂& ,
;
∂∂, -
OpenFileDialog
∏∏ 
openFileDialog
∏∏ )
=
∏∏* +
new
∏∏, /
OpenFileDialog
∏∏0 >
{
ππ 
Filter
∫∫ 
=
∫∫ 
$str
∫∫ b
,
∫∫b c
Title
ªª 
=
ªª 
Lang
ªª 
.
ªª $
SelectPuzzleImageTitle
ªª 3
}
ºº 
;
ºº 
if
ææ 
(
ææ 
openFileDialog
ææ 
.
ææ 

ShowDialog
ææ )
(
ææ) *
)
ææ* +
==
ææ, .
true
ææ/ 3
)
ææ3 4
{
øø 
IsBusy
¿¿ 
=
¿¿ 
true
¿¿ 
;
¿¿ 
try
¡¡ 
{
¬¬ 
byte
√√ 
[
√√ 
]
√√ 

imageBytes
√√ %
=
√√& '
File
√√( ,
.
√√, -
ReadAllBytes
√√- 9
(
√√9 :
openFileDialog
√√: H
.
√√H I
FileName
√√I Q
)
√√Q R
;
√√R S
string
ƒƒ 
fileName
ƒƒ #
=
ƒƒ$ %
Path
ƒƒ& *
.
ƒƒ* +
GetFileName
ƒƒ+ 6
(
ƒƒ6 7
openFileDialog
ƒƒ7 E
.
ƒƒE F
FileName
ƒƒF N
)
ƒƒN O
;
ƒƒO P
UploadResultDto
∆∆ #
uploadResult
∆∆$ 0
=
∆∆1 2
await
∆∆3 8
puzzleClient
∆∆9 E
.
∆∆E F$
uploadPuzzleImageAsync
∆∆F \
(
∆∆\ ]
SessionService
∆∆] k
.
∆∆k l
Username
∆∆l t
,
∆∆t u

imageBytes∆∆v Ä
,∆∆Ä Å
fileName∆∆Ç ä
)∆∆ä ã
;∆∆ã å
if
»» 
(
»» 
uploadResult
»» $
.
»»$ %
success
»»% ,
)
»», -
{
…… 
int
   
newPuzzleId
   '
=
  ( )
uploadResult
  * 6
.
  6 7
newPuzzleId
  7 B
;
  B C
var
ÀÀ 
newPuzzleInfo
ÀÀ )
=
ÀÀ* +
new
ÀÀ, /
PuzzleDisplayInfo
ÀÀ0 A
{
ÃÃ 
PuzzleId
ÕÕ $
=
ÕÕ% &
newPuzzleId
ÕÕ' 2
,
ÕÕ2 3
Name
ŒŒ  
=
ŒŒ! "
fileName
ŒŒ# +
,
ŒŒ+ ,
	ImagePath
œœ %
=
œœ& '
openFileDialog
œœ( 6
.
œœ6 7
FileName
œœ7 ?
}
–– 
;
–– 
AvailablePuzzles
—— (
.
——( )
Add
——) ,
(
——, -
newPuzzleInfo
——- :
)
——: ;
;
——; <!
executeSelectPuzzle
““ +
(
““+ ,
newPuzzleInfo
““, 9
)
““9 :
;
““: ;

MessageBox
”” "
.
””" #
Show
””# '
(
””' (
uploadResult
””( 4
.
””4 5
message
””5 <
,
””< =
Lang
””> B
.
””B C
UploadSuccessful
””C S
,
””S T
MessageBoxButton
””U e
.
””e f
OK
””f h
,
””h i
MessageBoxImage
””j y
.
””y z
Information””z Ö
)””Ö Ü
;””Ü á
}
‘‘ 
else
’’ 
{
÷÷ 

MessageBox
◊◊ "
.
◊◊" #
Show
◊◊# '
(
◊◊' (
uploadResult
◊◊( 4
.
◊◊4 5
message
◊◊5 <
,
◊◊< =
Lang
◊◊> B
.
◊◊B C
UploadFailed
◊◊C O
,
◊◊O P
MessageBoxButton
◊◊Q a
.
◊◊a b
OK
◊◊b d
,
◊◊d e
MessageBoxImage
◊◊f u
.
◊◊u v
Warning
◊◊v }
)
◊◊} ~
;
◊◊~ 
}
ÿÿ 
}
ŸŸ 
catch
⁄⁄ 
(
⁄⁄ 
	Exception
⁄⁄  
ex
⁄⁄! #
)
⁄⁄# $
{
€€ 

MessageBox
‹‹ 
.
‹‹ 
Show
‹‹ #
(
‹‹# $
$"
‹‹$ &
{
‹‹& '
Lang
‹‹' +
.
‹‹+ ,!
ErrorUploadingImage
‹‹, ?
}
‹‹? @
$str
‹‹@ B
{
‹‹B C
ex
‹‹C E
.
‹‹E F
Message
‹‹F M
}
‹‹M N
"
‹‹N O
,
‹‹O P
Lang
‹‹Q U
.
‹‹U V

ErrorTitle
‹‹V `
,
‹‹` a
MessageBoxButton
‹‹b r
.
‹‹r s
OK
‹‹s u
,
‹‹u v
MessageBoxImage‹‹w Ü
.‹‹Ü á
Error‹‹á å
)‹‹å ç
;‹‹ç é
}
›› 
finally
ﬁﬁ 
{
ﬂﬂ 
IsBusy
‡‡ 
=
‡‡ 
false
‡‡ "
;
‡‡" #
}
·· 
}
‚‚ 
}
„„ 	
private
ÂÂ 
async
ÂÂ 
Task
ÂÂ /
!executeConfirmAndCreateLobbyAsync
ÂÂ <
(
ÂÂ< =
)
ÂÂ= >
{
ÊÊ 	
if
ÁÁ 
(
ÁÁ 
!
ÁÁ &
canConfirmAndCreateLobby
ÁÁ )
(
ÁÁ) *
)
ÁÁ* +
)
ÁÁ+ ,
return
ÁÁ- 3
;
ÁÁ3 4
if
ËË 
(
ËË 
!
ËË -
MatchmakingServiceClientManager
ËË 0
.
ËË0 1
instance
ËË1 9
.
ËË9 :
EnsureConnected
ËË: I
(
ËËI J
)
ËËJ K
)
ËËK L
{
ÈÈ 

MessageBox
ÍÍ 
.
ÍÍ 
Show
ÍÍ 
(
ÍÍ  
Lang
ÍÍ  $
.
ÍÍ$ %&
CannotConnectMatchmaking
ÍÍ% =
,
ÍÍ= >
Lang
ÍÍ? C
.
ÍÍC D

ErrorTitle
ÍÍD N
,
ÍÍN O
MessageBoxButton
ÍÍP `
.
ÍÍ` a
OK
ÍÍa c
,
ÍÍc d
MessageBoxImage
ÍÍe t
.
ÍÍt u
Warning
ÍÍu |
)
ÍÍ| }
;
ÍÍ} ~
return
ÎÎ 
;
ÎÎ 
}
ÏÏ 
IsBusy
ÓÓ 
=
ÓÓ 
true
ÓÓ 
;
ÓÓ 
var
 
settings
 
=
 
new
 
LobbySettingsDto
 /
{
ÒÒ 
preloadedPuzzleId
ÚÚ !
=
ÚÚ" #
SelectedPuzzle
ÚÚ$ 2
.
ÚÚ2 3
PuzzleId
ÚÚ3 ;
,
ÚÚ; <
customPuzzleImage
ÛÛ !
=
ÛÛ" #
null
ÛÛ$ (
}
ÙÙ 
;
ÙÙ 
try
ˆˆ 
{
˜˜ $
LobbyCreationResultDto
¯¯ &
result
¯¯' -
=
¯¯. /
await
¯¯0 5
matchmakingClient
¯¯6 G
.
¯¯G H
createLobbyAsync
¯¯H X
(
¯¯X Y
SessionService
¯¯Y g
.
¯¯g h
Username
¯¯h p
,
¯¯p q
settings
¯¯r z
)
¯¯z {
;
¯¯{ |
if
˙˙ 
(
˙˙ 
result
˙˙ 
.
˙˙ 
success
˙˙ "
&&
˙˙# %
result
˙˙& ,
.
˙˙, -
initialLobbyState
˙˙- >
!=
˙˙? A
null
˙˙B F
)
˙˙F G
{
˚˚ 
var
¸¸ 
	lobbyPage
¸¸ !
=
¸¸" #
new
¸¸$ '
	LobbyPage
¸¸( 1
(
¸¸1 2
)
¸¸2 3
;
¸¸3 4
	lobbyPage
˝˝ 
.
˝˝ 
DataContext
˝˝ )
=
˝˝* +
new
˝˝, /
LobbyViewModel
˝˝0 >
(
˝˝> ?
result
˝˝? E
.
˝˝E F
initialLobbyState
˝˝F W
,
˝˝W X

navigateTo
˝˝Y c
,
˝˝c d
navigateBack
˝˝e q
)
˝˝q r
;
˝˝r s

navigateTo
˛˛ 
(
˛˛ 
	lobbyPage
˛˛ (
)
˛˛( )
;
˛˛) *
}
ˇˇ 
else
ÄÄ 
{
ÅÅ 

MessageBox
ÇÇ 
.
ÇÇ 
Show
ÇÇ #
(
ÇÇ# $
result
ÇÇ$ *
.
ÇÇ* +
message
ÇÇ+ 2
??
ÇÇ3 5
Lang
ÇÇ6 :
.
ÇÇ: ;!
FailedToCreateLobby
ÇÇ; N
,
ÇÇN O
Lang
ÇÇP T
.
ÇÇT U

ErrorTitle
ÇÇU _
,
ÇÇ_ `
MessageBoxButton
ÇÇa q
.
ÇÇq r
OK
ÇÇr t
,
ÇÇt u
MessageBoxImageÇÇv Ö
.ÇÇÖ Ü
ErrorÇÇÜ ã
)ÇÇã å
;ÇÇå ç
}
ÉÉ 
}
ÑÑ 
catch
ÖÖ 
(
ÖÖ 
	Exception
ÖÖ 
ex
ÖÖ 
)
ÖÖ  
{
ÜÜ 

MessageBox
áá 
.
áá 
Show
áá 
(
áá  
$"
áá  "
{
áá" #
Lang
áá# '
.
áá' (!
FailedToCreateLobby
áá( ;
}
áá; <
$str
áá< >
{
áá> ?
ex
áá? A
.
ááA B
Message
ááB I
}
ááI J
"
ááJ K
,
ááK L
Lang
ááM Q
.
ááQ R

ErrorTitle
ááR \
,
áá\ ]
MessageBoxButton
áá^ n
.
áán o
OK
ááo q
,
ááq r
MessageBoxImageáás Ç
.ááÇ É
ErrorááÉ à
)ááà â
;ááâ ä-
MatchmakingServiceClientManager
àà /
.
àà/ 0
instance
àà0 8
.
àà8 9

Disconnect
àà9 C
(
ààC D
)
ààD E
;
ààE F
}
ââ 
finally
ää 
{
ãã 
IsBusy
åå 
=
åå 
false
åå 
;
åå 
}
çç 
}
éé 	
private
êê 
void
êê 2
$raiseCanExecuteChangedForAllCommands
êê 9
(
êê9 :
)
êê: ;
{
ëë 	
Application
íí 
.
íí 
Current
íí 
.
íí  

Dispatcher
íí  *
.
íí* +
Invoke
íí+ 1
(
íí1 2
(
íí2 3
)
íí3 4
=>
íí5 7
CommandManager
íí8 F
.
ííF G(
InvalidateRequerySuggested
ííG a
(
íía b
)
ííb c
)
ííc d
;
ííd e
}
ìì 	
private
ïï 
void
ïï 
setBusy
ïï 
(
ïï 
bool
ïï !
value
ïï" '
)
ïï' (
{
ññ 	
if
óó 
(
óó 
isBusyValue
óó 
!=
óó 
value
óó $
)
óó$ %
{
òò 
isBusyValue
ôô 
=
ôô 
value
ôô #
;
ôô# $
OnPropertyChanged
öö !
(
öö! "
nameof
öö" (
(
öö( )
IsBusy
öö) /
)
öö/ 0
)
öö0 1
;
öö1 22
$raiseCanExecuteChangedForAllCommands
õõ 4
(
õõ4 5
)
õõ5 6
;
õõ6 7
}
úú 
}
ùù 	
}
ûû 
}üü úO
óC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\ViewModel\Main\SelectAvatarViewModel.cs
	namespace 	
MindWeaveClient
 
. 
	ViewModel #
.# $
Main$ (
{ 
public 

class 
Avatar 
: 
BaseViewModel '
{ 
private 
bool 
isSelectedValue $
;$ %
public 
string 
	ImagePath 
{  !
get" %
;% &
set' *
;* +
}, -
public 
bool 

IsSelected 
{ 	
get 
=> 
isSelectedValue "
;" #
set 
{ 
isSelectedValue !
=" #
value$ )
;) *
OnPropertyChanged+ <
(< =
)= >
;> ?
}@ A
} 	
} 
public 

class !
SelectAvatarViewModel &
:' (
BaseViewModel) 6
{ 
private 
readonly 
Action 
navigateBack  ,
;, -
private  
ObservableCollection $
<$ %
Avatar% +
>+ ,!
availableAvatarsValue- B
;B C
private 
Avatar 
selectedAvatarValue *
;* +
private 
bool 
isBusyValue  
;  !
public  
ObservableCollection #
<# $
Avatar$ *
>* +
AvailableAvatars, <
{= >
get? B
=>C E!
availableAvatarsValueF [
;[ \
set] `
{a b!
availableAvatarsValuec x
=y z
value	{ Ä
;
Ä Å
OnPropertyChanged
Ç ì
(
ì î
)
î ï
;
ï ñ
}
ó ò
}
ô ö
public   
Avatar   
SelectedAvatar   $
{  % &
get  ' *
=>  + -
selectedAvatarValue  . A
;  A B
set  C F
{  G H
selectedAvatarValue  I \
=  ] ^
value  _ d
;  d e
OnPropertyChanged  f w
(  w x
)  x y
;  y z
(  { |
(  | }
RelayCommand	  } â
)
  â ä"
SaveSelectionCommand
  ä û
)
  û ü
.
  ü †$
RaiseCanExecuteChanged
  † ∂
(
  ∂ ∑
)
  ∑ ∏
;
  ∏ π
}
  ∫ ª
}
  º Ω
public!! 
bool!! 
IsBusy!! 
{!! 
get!!  
=>!!! #
isBusyValue!!$ /
;!!/ 0
private!!1 8
set!!9 <
{!!= >
setBusy!!? F
(!!F G
value!!G L
)!!L M
;!!M N
}!!O P
}!!Q R
public## 
ICommand##  
SaveSelectionCommand## ,
{##- .
get##/ 2
;##2 3
}##4 5
public$$ 
ICommand$$ 
CancelCommand$$ %
{$$& '
get$$( +
;$$+ ,
}$$- .
public&& !
SelectAvatarViewModel&& $
(&&$ %
Action&&% +
navigateBack&&, 8
)&&8 9
{'' 	
this(( 
.(( 
navigateBack(( 
=(( 
navigateBack((  ,
;((, -
CancelCommand)) 
=)) 
new)) 
RelayCommand))  ,
()), -
p))- .
=>))/ 1
this))2 6
.))6 7
navigateBack))7 C
?))C D
.))D E
Invoke))E K
())K L
)))L M
,))M N
p))O P
=>))Q S
!))T U
IsBusy))U [
)))[ \
;))\ ] 
SaveSelectionCommand**  
=**! "
new**# &
RelayCommand**' 3
(**3 4
async**4 9
p**: ;
=>**< >
await**? D
saveSelection**E R
(**R S
)**S T
,**T U
p**V W
=>**X Z
canSave**[ b
(**b c
)**c d
)**d e
;**e f 
loadAvailableAvatars,,  
(,,  !
),,! "
;,," #
}-- 	
private// 
void//  
loadAvailableAvatars// )
(//) *
)//* +
{00 	
AvailableAvatars11 
=11 
new11 " 
ObservableCollection11# 7
<117 8
Avatar118 >
>11> ?
(11? @
)11@ A
;11A B
var22 
avatarPaths22 
=22 
new22 !
string22" (
[22( )
]22) *
{33 
$str44 =
,44= >
$str55 ;
,55; <
$str66 <
,66< =
$str77 :
,77: ;
$str88 ;
,88; <
$str99 ;
,99; <
}:: 
;:: 
foreach<< 
(<< 
var<< 
path<< 
in<<  
avatarPaths<<! ,
)<<, -
{== 
AvailableAvatars>>  
.>>  !
Add>>! $
(>>$ %
new>>% (
Avatar>>) /
{>>0 1
	ImagePath>>2 ;
=>>< =
path>>> B
}>>C D
)>>D E
;>>E F
}?? 
varAA 
currentAvatarAA 
=AA 
AvailableAvatarsAA  0
.AA0 1
FirstOrDefaultAA1 ?
(AA? @
aAA@ A
=>AAB D
aAAE F
.AAF G
	ImagePathAAG P
.AAP Q
EqualsAAQ W
(AAW X
SessionServiceAAX f
.AAf g

AvatarPathAAg q
,AAq r
StringComparison	AAs É
.
AAÉ Ñ
OrdinalIgnoreCase
AAÑ ï
)
AAï ñ
)
AAñ ó
;
AAó ò
ifBB 
(BB 
currentAvatarBB 
!=BB  
nullBB! %
)BB% &
{CC 
currentAvatarDD 
.DD 

IsSelectedDD (
=DD) *
trueDD+ /
;DD/ 0
SelectedAvatarEE 
=EE  
currentAvatarEE! .
;EE. /
}FF 
}GG 	
privateII 
boolII 
canSaveII 
(II 
)II 
=>II !
SelectedAvatarII" 0
!=II1 3
nullII4 8
&&II9 ;
!II< =
IsBusyII= C
;IIC D
privateLL 
asyncLL 
TaskLL 
saveSelectionLL (
(LL( )
)LL) *
{MM 	
ifNN 
(NN 
!NN 
canSaveNN 
(NN 
)NN 
)NN 
returnNN "
;NN" #
setBusyPP 
(PP 
truePP 
)PP 
;PP 
tryQQ 
{RR 
varSS 
clientSS 
=SS 
newSS   
ProfileManagerClientSS! 5
(SS5 6
)SS6 7
;SS7 8
varTT 
resultTT 
=TT 
awaitTT "
clientTT# )
.TT) *!
updateAvatarPathAsyncTT* ?
(TT? @
SessionServiceTT@ N
.TTN O
UsernameTTO W
,TTW X
SelectedAvatarTTY g
.TTg h
	ImagePathTTh q
)TTq r
;TTr s
ifVV 
(VV 
resultVV 
.VV 
successVV "
)VV" #
{WW 
SessionServiceXX "
.XX" #
UpdateAvatarPathXX# 3
(XX3 4
SelectedAvatarXX4 B
.XXB C
	ImagePathXXC L
)XXL M
;XXM N

MessageBoxYY 
.YY 
ShowYY #
(YY# $
resultYY$ *
.YY* +
messageYY+ 2
,YY2 3
LangYY4 8
.YY8 9
InfoMsgTitleSuccessYY9 L
,YYL M
MessageBoxButtonYYN ^
.YY^ _
OKYY_ a
,YYa b
MessageBoxImageYYc r
.YYr s
InformationYYs ~
)YY~ 
;	YY Ä
navigateBackZZ  
?ZZ  !
.ZZ! "
InvokeZZ" (
(ZZ( )
)ZZ) *
;ZZ* +
}[[ 
else\\ 
{]] 

MessageBox^^ 
.^^ 
Show^^ #
(^^# $
result^^$ *
.^^* +
message^^+ 2
,^^2 3
Lang^^4 8
.^^8 9

ErrorTitle^^9 C
,^^C D
MessageBoxButton^^E U
.^^U V
OK^^V X
,^^X Y
MessageBoxImage^^Z i
.^^i j
Error^^j o
)^^o p
;^^p q
}__ 
}`` 
catchaa 
(aa 
	Exceptionaa 
exaa 
)aa  
{bb 

MessageBoxcc 
.cc 
Showcc 
(cc  
$"cc  "
{cc" #
excc# %
.cc% &
Messagecc& -
}cc- .
"cc. /
,cc/ 0
Langcc1 5
.cc5 6

ErrorTitlecc6 @
,cc@ A
MessageBoxButtonccB R
.ccR S
OKccS U
,ccU V
MessageBoxImageccW f
.ccf g
Errorccg l
)ccl m
;ccm n
}dd 
finallyee 
{ff 
setBusygg 
(gg 
falsegg 
)gg 
;gg 
}hh 
}ii 	
privatekk 
voidkk 
setBusykk 
(kk 
boolkk !
valuekk" '
)kk' (
{ll 	
isBusyValuemm 
=mm 
valuemm 
;mm  
OnPropertyChangednn 
(nn 
nameofnn $
(nn$ %
IsBusynn% +
)nn+ ,
)nn, -
;nn- .
Applicationoo 
.oo 
Currentoo 
?oo  
.oo  !

Dispatcheroo! +
?oo+ ,
.oo, -
Invokeoo- 3
(oo3 4
(oo4 5
)oo5 6
=>oo7 9
CommandManageroo: H
.ooH I&
InvalidateRequerySuggestedooI c
(ooc d
)ood e
)ooe f
;oof g
}pp 	
}qq 
}rr ’f
íC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\ViewModel\Main\ProfileViewModel.cs
	namespace		 	
MindWeaveClient		
 
.		 
	ViewModel		 #
.		# $
Main		$ (
{

 
public 

class 
ProfileViewModel !
:" #
BaseViewModel$ 1
{ 
private 
readonly 
Action 
navigateBack  ,
;, -
private 
readonly 
Action 
navigateToEdit  .
;. /
private 
string 
welcomeMessageValue *
;* +
private 
string 
usernameValue $
;$ %
private 
string 
avatarSourceValue (
;( )
private 
string 
firstNameValue %
;% &
private 
string 
lastNameValue $
;$ %
private 
string 
dateOfBirthValue '
;' (
private 
string 
genderValue "
;" #
private 
int !
puzzlesCompletedValue )
;) *
private 
int 
puzzlesWonValue #
;# $
private 
string 
totalPlaytimeValue )
;) *
private 
int 
highestScoreValue %
;% &
private  
ObservableCollection $
<$ %
AchievementDto% 3
>3 4
achievementsValue5 F
;F G
public 
string 
WelcomeMessage $
{% &
get' *
=>+ -
welcomeMessageValue. A
;A B
setC F
{G H
welcomeMessageValueI \
=] ^
value_ d
;d e
OnPropertyChangedf w
(w x
)x y
;y z
}{ |
}} ~
public 
string 
Username 
{  
get! $
=>% '
usernameValue( 5
;5 6
set7 :
{; <
usernameValue= J
=K L
valueM R
;R S
OnPropertyChangedT e
(e f
)f g
;g h
}i j
}k l
public 
string 
AvatarSource "
{# $
get% (
=>) +
avatarSourceValue, =
;= >
set? B
{C D
avatarSourceValueE V
=W X
valueY ^
;^ _
OnPropertyChanged` q
(q r
)r s
;s t
}u v
}w x
public   
string   
	FirstName   
{    !
get  " %
=>  & (
firstNameValue  ) 7
;  7 8
set  9 <
{  = >
firstNameValue  ? M
=  N O
value  P U
;  U V
OnPropertyChanged  W h
(  h i
)  i j
;  j k
}  l m
}  n o
public!! 
string!! 
LastName!! 
{!!  
get!!! $
=>!!% '
lastNameValue!!( 5
;!!5 6
set!!7 :
{!!; <
lastNameValue!!= J
=!!K L
value!!M R
;!!R S
OnPropertyChanged!!T e
(!!e f
)!!f g
;!!g h
}!!i j
}!!k l
public"" 
string"" 
DateOfBirth"" !
{""" #
get""$ '
=>""( *
dateOfBirthValue""+ ;
;""; <
set""= @
{""A B
dateOfBirthValue""C S
=""T U
value""V [
;""[ \
OnPropertyChanged""] n
(""n o
)""o p
;""p q
}""r s
}""t u
public## 
string## 
Gender## 
{## 
get## "
=>### %
genderValue##& 1
;##1 2
set##3 6
{##7 8
genderValue##9 D
=##E F
value##G L
;##L M
OnPropertyChanged##N _
(##_ `
)##` a
;##a b
}##c d
}##e f
public$$ 
int$$ 
PuzzlesCompleted$$ #
{$$$ %
get$$& )
=>$$* ,!
puzzlesCompletedValue$$- B
;$$B C
set$$D G
{$$H I!
puzzlesCompletedValue$$J _
=$$` a
value$$b g
;$$g h
OnPropertyChanged$$i z
($$z {
)$${ |
;$$| }
}$$~ 
}
$$Ä Å
public%% 
int%% 

PuzzlesWon%% 
{%% 
get%%  #
=>%%$ &
puzzlesWonValue%%' 6
;%%6 7
set%%8 ;
{%%< =
puzzlesWonValue%%> M
=%%N O
value%%P U
;%%U V
OnPropertyChanged%%W h
(%%h i
)%%i j
;%%j k
}%%l m
}%%n o
public&& 
string&& 
TotalPlaytime&& #
{&&$ %
get&&& )
=>&&* ,
totalPlaytimeValue&&- ?
;&&? @
set&&A D
{&&E F
totalPlaytimeValue&&G Y
=&&Z [
value&&\ a
;&&a b
OnPropertyChanged&&c t
(&&t u
)&&u v
;&&v w
}&&x y
}&&z {
public'' 
int'' 
HighestScore'' 
{''  !
get''" %
=>''& (
highestScoreValue'') :
;'': ;
set''< ?
{''@ A
highestScoreValue''B S
=''T U
value''V [
;''[ \
OnPropertyChanged''] n
(''n o
)''o p
;''p q
}''r s
}''t u
public((  
ObservableCollection(( #
<((# $
AchievementDto(($ 2
>((2 3
Achievements((4 @
{((A B
get((C F
=>((G I
achievementsValue((J [
;(([ \
set((] `
{((a b
achievementsValue((c t
=((u v
value((w |
;((| }
OnPropertyChanged	((~ è
(
((è ê
)
((ê ë
;
((ë í
}
((ì î
}
((ï ñ
public** 
ICommand** 
BackCommand** #
{**$ %
get**& )
;**) *
}**+ ,
public++ 
ICommand++ 
EditProfileCommand++ *
{+++ ,
get++- 0
;++0 1
}++2 3
public-- 
ProfileViewModel-- 
(--  
Action--  &
navigateBack--' 3
,--3 4
Action--5 ;
navigateToEdit--< J
)--J K
{.. 	
this// 
.// 
navigateBack// 
=// 
navigateBack//  ,
;//, -
this00 
.00 
navigateToEdit00 
=00  !
navigateToEdit00" 0
;000 1
BackCommand22 
=22 
new22 
RelayCommand22 *
(22* +
p22+ ,
=>22- /
navigateBack220 <
?22< =
.22= >
Invoke22> D
(22D E
)22E F
)22F G
;22G H
EditProfileCommand44 
=44  
new44! $
RelayCommand44% 1
(441 2
p442 3
=>444 6
navigateToEdit447 E
?44E F
.44F G
Invoke44G M
(44M N
)44N O
)44O P
;44P Q
Username66 
=66 
SessionService66 %
.66% &
Username66& .
??66/ 1
$str662 >
;66> ?
WelcomeMessage77 
=77 
string77 #
.77# $
Format77$ *
(77* +
$str77+ 5
,775 6
Lang777 ;
.77; <
ProfileLbHi77< G
.77G H
TrimEnd77H O
(77O P
$char77P S
)77S T
,77T U
Username77V ^
.77^ _
ToUpper77_ f
(77f g
)77g h
)77h i
;77i j
AvatarSource88 
=88 
SessionService88 )
.88) *

AvatarPath88* 4
??885 7
$str888 e
;88e f
Achievements99 
=99 
new99  
ObservableCollection99 3
<993 4
AchievementDto994 B
>99B C
(99C D
)99D E
;99E F
loadProfileData;; 
(;; 
);; 
;;; 
}<< 	
private>> 
async>> 
void>> 
loadProfileData>> *
(>>* +
)>>+ ,
{?? 	
if@@ 
(@@ 
string@@ 
.@@ 
IsNullOrEmpty@@ $
(@@$ %
SessionService@@% 3
.@@3 4
Username@@4 <
)@@< =
)@@= >
return@@? E
;@@E F
tryBB 
{CC 
varDD 
clientDD 
=DD 
newDD   
ProfileManagerClientDD! 5
(DD5 6
)DD6 7
;DD7 8 
PlayerProfileViewDtoEE $
profileDataEE% 0
=EE1 2
awaitEE3 8
clientEE9 ?
.EE? @%
getPlayerProfileViewAsyncEE@ Y
(EEY Z
SessionServiceEEZ h
.EEh i
UsernameEEi q
)EEq r
;EEr s
ifGG 
(GG 
profileDataGG 
!=GG  "
nullGG# '
)GG' (
{HH 
UsernameII 
=II 
profileDataII *
.II* +
usernameII+ 3
;II3 4
WelcomeMessageJJ "
=JJ# $
stringJJ% +
.JJ+ ,
FormatJJ, 2
(JJ2 3
$strJJ3 =
,JJ= >
LangJJ? C
.JJC D
ProfileLbHiJJD O
.JJO P
TrimEndJJP W
(JJW X
$charJJX [
)JJ[ \
,JJ\ ]
profileDataJJ^ i
.JJi j
usernameJJj r
.JJr s
ToUpperJJs z
(JJz {
)JJ{ |
)JJ| }
;JJ} ~
AvatarSourceKK  
=KK! "
profileDataKK# .
.KK. /

avatarPathKK/ 9
??KK: <
$strKK= j
;KKj k
	FirstNameMM 
=MM 
profileDataMM  +
.MM+ ,
	firstNameMM, 5
;MM5 6
LastNameNN 
=NN 
profileDataNN *
.NN* +
lastNameNN+ 3
;NN3 4
DateOfBirthOO 
=OO  !
profileDataOO" -
.OO- .
dateOfBirthOO. 9
?OO9 :
.OO: ;
ToStringOO; C
(OOC D
$strOOD P
)OOP Q
??OOR T
$strOOU d
;OOd e
GenderPP 
=PP 
profileDataPP (
.PP( )
genderPP) /
??PP0 2
$strPP3 B
;PPB C
ifRR 
(RR 
profileDataRR #
.RR# $
statsRR$ )
!=RR* ,
nullRR- 1
)RR1 2
{SS 
PuzzlesCompletedTT (
=TT) *
profileDataTT+ 6
.TT6 7
statsTT7 <
.TT< =
puzzlesCompletedTT= M
;TTM N

PuzzlesWonUU "
=UU# $
profileDataUU% 0
.UU0 1
statsUU1 6
.UU6 7

puzzlesWonUU7 A
;UUA B
TotalPlaytimeVV %
=VV& '
$"VV( *
{VV* +
profileDataVV+ 6
.VV6 7
statsVV7 <
.VV< =
totalPlaytimeVV= J
.VVJ K
HoursVVK P
}VVP Q
$strVVQ S
{VVS T
profileDataVVT _
.VV_ `
statsVV` e
.VVe f
totalPlaytimeVVf s
.VVs t
MinutesVVt {
}VV{ |
$strVV| }
"VV} ~
;VV~ 
HighestScoreWW $
=WW% &
profileDataWW' 2
.WW2 3
statsWW3 8
.WW8 9
highestScoreWW9 E
;WWE F
}XX 
ifZZ 
(ZZ 
profileDataZZ #
.ZZ# $
achievementsZZ$ 0
!=ZZ1 3
nullZZ4 8
)ZZ8 9
{[[ 
Achievements\\ $
=\\% &
new\\' * 
ObservableCollection\\+ ?
<\\? @
AchievementDto\\@ N
>\\N O
(\\O P
profileData\\P [
.\\[ \
achievements\\\ h
)\\h i
;\\i j
}]] 
}^^ 
}__ 
catch`` 
(`` 
	Exception`` 
ex`` 
)``  
{aa 

MessageBoxbb 
.bb 
Showbb 
(bb  
$"bb  "
{bb" #
Langbb# '
.bb' ($
ErrorFailedToLoadProfilebb( @
}bb@ A
$strbbA C
{bbC D
exbbD F
.bbF G
MessagebbG N
}bbN O
"bbO P
,bbP Q
LangbbR V
.bbV W

ErrorTitlebbW a
,bba b
MessageBoxButtonbbc s
.bbs t
OKbbt v
,bbv w
MessageBoxImage	bbx á
.
bbá à
Error
bbà ç
)
bbç é
;
bbé è
}cc 
}dd 	
}ee 
}ff Üt
ìC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\ViewModel\Main\MainMenuViewModel.cs
	namespace 	
MindWeaveClient
 
. 
	ViewModel #
.# $
Main$ (
{ 
public 

class 
MainMenuViewModel "
:# $
BaseViewModel% 2
{ 
private 
readonly 
Action 
<  
Page  $
>$ %

navigateTo& 0
;0 1
private 
readonly 
Page 
mainMenuPage *
;* +
public 
string 
PlayerUsername $
{% &
get' *
;* +
}, -
public 
string 
PlayerAvatarPath &
{' (
get) ,
;, -
}. /
public 
ICommand 
ProfileCommand &
{' (
get) ,
;, -
}. /
public 
ICommand 
CreateLobbyCommand *
{+ ,
get- 0
;0 1
}2 3
public 
ICommand 
SocialCommand %
{& '
get( +
;+ ,
}- .
public 
ICommand 
SettingsCommand '
{( )
get* -
;- .
}/ 0
public 
ICommand 
JoinLobbyCommand (
{) *
get+ .
;. /
}0 1
private 
string 
joinLobbyCodeValue )
=* +
string, 2
.2 3
Empty3 8
;8 9
public 
string 
JoinLobbyCode #
{   	
get!! 
=>!! 
joinLobbyCodeValue!! %
;!!% &
set"" 
{## 
joinLobbyCodeValue$$ "
=$$# $
value$$% *
?$$* +
.$$+ ,
ToUpper$$, 3
($$3 4
)$$4 5
.$$5 6
Trim$$6 :
($$: ;
)$$; <
??$$= ?
string$$@ F
.$$F G
Empty$$G L
;$$L M
if%% 
(%% 
joinLobbyCodeValue%% &
.%%& '
Length%%' -
>%%. /
$num%%0 1
)%%1 2
{%%3 4
joinLobbyCodeValue%%5 G
=%%H I
joinLobbyCodeValue%%J \
.%%\ ]
	Substring%%] f
(%%f g
$num%%g h
,%%h i
$num%%j k
)%%k l
;%%l m
}%%n o
OnPropertyChanged&& !
(&&! "
)&&" #
;&&# $
OnPropertyChanged'' !
(''! "
nameof''" (
(''( )
CanJoinLobby'') 5
)''5 6
)''6 7
;''7 8
((( 
((( 
RelayCommand(( 
)(( 
JoinLobbyCommand(( /
)((/ 0
.((0 1"
RaiseCanExecuteChanged((1 G
(((G H
)((H I
;((I J
})) 
}** 	
public++ 
bool++ 
CanJoinLobby++  
=>++! #
!++$ %
IsBusy++% +
&&++, .
!++/ 0
string++0 6
.++6 7
IsNullOrWhiteSpace++7 I
(++I J
JoinLobbyCode++J W
)++W X
&&++Y [
JoinLobbyCode++\ i
.++i j
Length++j p
==++q s
$num++t u
;++u v
private-- 
bool-- 
isBusyValue--  
;--  !
public.. 
bool.. 
IsBusy.. 
{// 	
get00 
=>00 
isBusyValue00 
;00 
set11 
{22 
isBusyValue33 
=33 
value33 #
;33# $
OnPropertyChanged44 !
(44! "
)44" #
;44# $
(55 
(55 
RelayCommand55 
)55 
CreateLobbyCommand55 1
)551 2
.552 3"
RaiseCanExecuteChanged553 I
(55I J
)55J K
;55K L
(66 
(66 
RelayCommand66 
)66 
JoinLobbyCommand66 /
)66/ 0
.660 1"
RaiseCanExecuteChanged661 G
(66G H
)66H I
;66I J
OnPropertyChanged77 !
(77! "
nameof77" (
(77( )
CanJoinLobby77) 5
)775 6
)776 7
;777 8
}88 
}99 	
private;; $
MatchmakingManagerClient;; (
matchmakingProxy;;) 9
=>;;: <+
MatchmakingServiceClientManager;;= \
.;;\ ]
instance;;] e
.;;e f
proxy;;f k
;;;k l
public== 
MainMenuViewModel==  
(==  !
Action==! '
<==' (
Page==( ,
>==, -

navigateTo==. 8
,==8 9
Page==: >
mainMenuPage==? K
)==K L
{>> 	
this?? 
.?? 

navigateTo?? 
=?? 

navigateTo?? (
;??( )
this@@ 
.@@ 
mainMenuPage@@ 
=@@ 
mainMenuPage@@  ,
;@@, -
PlayerUsernameBB 
=BB 
SessionServiceBB +
.BB+ ,
UsernameBB, 4
;BB4 5
PlayerAvatarPathCC 
=CC 
SessionServiceCC -
.CC- .

AvatarPathCC. 8
??CC9 ;
$strCC< i
;CCi j
ProfileCommandEE 
=EE 
newEE  
RelayCommandEE! -
(EE- .
pEE. /
=>EE0 2
executeGoToProfileEE3 E
(EEE F
)EEF G
)EEG H
;EEH I
CreateLobbyCommandFF 
=FF  
newFF! $
RelayCommandFF% 1
(FF1 2
pFF2 3
=>FF4 6&
executeGoToPuzzleSelectionFF7 Q
(FFQ R
)FFR S
,FFS T
pFFU V
=>FFW Y
!FFZ [
IsBusyFF[ a
)FFa b
;FFb c
SocialCommandGG 
=GG 
newGG 
RelayCommandGG  ,
(GG, -
pGG- .
=>GG/ 1
executeGoToSocialGG2 C
(GGC D
)GGD E
)GGE F
;GGF G
SettingsCommandHH 
=HH 
newHH !
RelayCommandHH" .
(HH. /
pHH/ 0
=>HH1 3
executeShowSettingsHH4 G
(HHG H
)HHH I
,HHI J
pHHK L
=>HHM O
!HHP Q
IsBusyHHQ W
)HHW X
;HHX Y
JoinLobbyCommandII 
=II 
newII "
RelayCommandII# /
(II/ 0
asyncII0 5
pII6 7
=>II8 :
awaitII; @!
executeJoinLobbyAsyncIIA V
(IIV W
)IIW X
,IIX Y
pIIZ [
=>II\ ^
CanJoinLobbyII_ k
)IIk l
;IIl m
ConsoleKK 
.KK 
	WriteLineKK 
(KK 
$"KK  
$strKK  L
{KKL M
PlayerAvatarPathKKM ]
}KK] ^
"KK^ _
)KK_ `
;KK` a
}LL 	
privateMM 
voidMM &
executeGoToPuzzleSelectionMM /
(MM/ 0
)MM0 1
{NN 	
varOO 
selectionPageOO 
=OO 
newOO  #
SelectionPuzzlePageOO$ 7
(OO7 8
)OO8 9
;OO9 :
selectionPagePP 
.PP 
DataContextPP %
=PP& '
newPP( +$
SelectionPuzzleViewModelPP, D
(PPD E

navigateToQQ 
,QQ 
(RR 
)RR 
=>RR 

navigateToRR  
(RR  !
mainMenuPageRR! -
)RR- .
)SS 
;SS 

navigateToTT 
(TT 
selectionPageTT $
)TT$ %
;TT% &
}UU 	
privateVV 
asyncVV 
TaskVV !
executeJoinLobbyAsyncVV 0
(VV0 1
)VV1 2
{WW 	
ifXX 
(XX 
!XX 
CanJoinLobbyXX 
)XX 
returnXX %
;XX% &
ifYY 
(YY 
!YY 
RegexYY 
.YY 
IsMatchYY 
(YY 
JoinLobbyCodeYY ,
,YY, -
$strYY. =
)YY= >
)YY> ?
{YY@ A
returnYYJ P
;YYP Q
}YYR S
IsBusy[[ 
=[[ 
true[[ 
;[[ 
try\\ 
{]] 
if^^ 
(^^ 
!^^ +
MatchmakingServiceClientManager^^ 4
.^^4 5
instance^^5 =
.^^= >
EnsureConnected^^> M
(^^M N
)^^N O
)^^O P
{^^Q R
return^^[ a
;^^a b
}^^c d
matchmakingProxy``  
.``  !
	joinLobby``! *
(``* +
SessionService``+ 9
.``9 :
Username``: B
,``B C
JoinLobbyCode``D Q
)``Q R
;``R S

MessageBoxaa 
.aa 
Showaa 
(aa  
$"aa  "
$straa" ;
{aa; <
JoinLobbyCodeaa< I
}aaI J
$straaJ M
"aaM N
,aaN O
$straaP Y
,aaY Z
MessageBoxButtonaa[ k
.aak l
OKaal n
,aan o
MessageBoxImageaap 
.	aa Ä
Information
aaÄ ã
)
aaã å
;
aaå ç
varcc 
	lobbyPagecc 
=cc 
newcc  #
	LobbyPagecc$ -
(cc- .
)cc. /
;cc/ 0
	lobbyPagedd 
.dd 
DataContextdd %
=dd& '
newdd( +
LobbyViewModeldd, :
(dd: ;
nullee 
,ee 

navigateToff 
,ff 
(gg 
)gg 
=>gg 

navigateTogg $
(gg$ %
mainMenuPagegg% 1
)gg1 2
)hh 
;hh 

navigateToii 
(ii 
	lobbyPageii $
)ii$ %
;ii% &
}jj 
catchkk 
(kk 
	Exceptionkk 
exkk 
)kk  
{ll 

MessageBoxmm 
.mm 
Showmm 
(mm  
$"mm  "
$strmm" I
{mmI J
exmmJ L
.mmL M
MessagemmM T
}mmT U
"mmU V
,mmV W
$strmmX _
,mm_ `
MessageBoxButtonmma q
.mmq r
OKmmr t
,mmt u
MessageBoxImage	mmv Ö
.
mmÖ Ü
Error
mmÜ ã
)
mmã å
;
mmå ç+
MatchmakingServiceClientManagernn /
.nn/ 0
instancenn0 8
.nn8 9

Disconnectnn9 C
(nnC D
)nnD E
;nnE F
}oo 
finallypp 
{qq 
IsBusyrr 
=rr 
falserr 
;rr 
}ss 
}tt 	
privateww 
voidww 
executeGoToProfileww '
(ww' (
)ww( )
{xx 	
varyy 
profilePageyy 
=yy 
newyy !
ProfilePageyy" -
(yy- .
)yy. /
;yy/ 0
profilePagezz 
.zz 
DataContextzz #
=zz$ %
newzz& )
ProfileViewModelzz* :
(zz: ;
({{ 
){{ 
=>{{ 

navigateTo{{  
({{  !
mainMenuPage{{! -
){{- .
,{{. /
(|| 
)|| 
=>|| "
executeGoToEditProfile|| ,
(||, -
)||- .
)}} 
;}} 

navigateTo~~ 
(~~ 
profilePage~~ "
)~~" #
;~~# $
} 	
private
ÅÅ 
void
ÅÅ 
executeGoToSocial
ÅÅ &
(
ÅÅ& '
)
ÅÅ' (
{
ÅÅ) *

navigateTo
ÇÇ 
(
ÇÇ 
new
ÇÇ 

SocialPage
ÇÇ %
(
ÇÇ% &
)
ÇÇ& '
)
ÇÇ' (
;
ÇÇ( )
}
ÉÉ 	
private
ÖÖ 
void
ÖÖ $
executeGoToEditProfile
ÖÖ +
(
ÖÖ+ ,
)
ÖÖ, -
{
ÜÜ 	
var
áá 
editProfilePage
áá 
=
áá  !
new
áá" %
EditProfilePage
áá& 5
(
áá5 6
)
áá6 7
;
áá7 8
editProfilePage
àà 
.
àà 
DataContext
àà '
=
àà( )
new
àà* -"
EditProfileViewModel
àà. B
(
ààB C
(
ââ 
)
ââ 
=>
ââ  
executeGoToProfile
ââ (
(
ââ( )
)
ââ) *
,
ââ* +
(
ää 
)
ää 
=>
ää %
executeGoToSelectAvatar
ää -
(
ää- .
)
ää. /
)
ãã 
;
ãã 

navigateTo
åå 
(
åå 
editProfilePage
åå &
)
åå& '
;
åå' (
}
çç 	
private
èè 
void
èè %
executeGoToSelectAvatar
èè ,
(
èè, -
)
èè- .
{
êê 	
var
ëë 
selectAvatarPage
ëë  
=
ëë! "
new
ëë# &
SelectAvatarPage
ëë' 7
(
ëë7 8
)
ëë8 9
;
ëë9 :
selectAvatarPage
íí 
.
íí 
DataContext
íí (
=
íí) *
new
íí+ .#
SelectAvatarViewModel
íí/ D
(
ííD E
(
ìì 
)
ìì 
=>
ìì $
executeGoToEditProfile
ìì ,
(
ìì, -
)
ìì- .
)
îî 
;
îî 

navigateTo
ïï 
(
ïï 
selectAvatarPage
ïï '
)
ïï' (
;
ïï( )
}
ññ 	
private
óó 
void
óó !
executeShowSettings
óó (
(
óó( )
)
óó) *
{
òò 	
var
ôô 
settingsWindow
ôô 
=
ôô  
new
ôô! $
SettingsWindow
ôô% 3
(
ôô3 4
)
ôô4 5
;
ôô5 6
settingsWindow
öö 
.
öö 
Owner
öö  
=
öö! "
Application
öö# .
.
öö. /
Current
öö/ 6
.
öö6 7

MainWindow
öö7 A
;
ööA B
bool
õõ 
?
õõ 
result
õõ 
=
õõ 
settingsWindow
õõ )
.
õõ) *

ShowDialog
õõ* 4
(
õõ4 5
)
õõ5 6
;
õõ6 7
if
ùù 
(
ùù 
result
ùù 
==
ùù 
true
ùù 
)
ùù 
{
ûû 
Console
üü 
.
üü 
	WriteLine
üü !
(
üü! "
$str
üü" 3
)
üü3 4
;
üü4 5
}
†† 
else
°° 
{
¢¢ 
Console
££ 
.
££ 
	WriteLine
££ "
(
££" #
$str
££# 7
)
££7 8
;
££8 9
}
§§ 
}
•• 	
}
ßß 
}®® ù”
ñC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\ViewModel\Main\EditProfileViewModel.cs
	namespace 	
MindWeaveClient
 
. 
	ViewModel #
.# $
Main$ (
{ 
public 

class  
EditProfileViewModel %
:& '
BaseViewModel( 5
{ 
private 
readonly 
Action 
navigateBack  ,
;, -
private 
readonly 
Action "
navigateToSelectAvatar  6
;6 7
private  
ProfileManagerClient $
profileClient% 2
;2 3
private 
string 
firstNameValue %
;% &
private 
string 
lastNameValue $
;$ %
private 
DateTime 
? 
dateOfBirthValue *
;* +
private 
	GenderDto 
selectedGenderValue -
;- .
private  
ObservableCollection $
<$ %
	GenderDto% .
>. /
gendersValue0 <
;< =
private 
string 
avatarSourceValue (
;( )
private 
bool /
#isChangePasswordSectionVisibleValue 8
;8 9
private 
string  
currentPasswordValue +
;+ ,
private 
string 
newPasswordValue '
;' (
private 
string  
confirmPasswordValue +
;+ ,
private 
bool 
isBusyValue  
;  !
public   
string   
	FirstName   
{    !
get  " %
=>  & (
firstNameValue  ) 7
;  7 8
set  9 <
{  = >
firstNameValue  ? M
=  N O
value  P U
;  U V
OnPropertyChanged  W h
(  h i
)  i j
;  j k#
raiseCanExecuteChanged	  l Ç
(
  Ç É
)
  É Ñ
;
  Ñ Ö
}
  Ü á
}
  à â
public!! 
string!! 
LastName!! 
{!!  
get!!! $
=>!!% '
lastNameValue!!( 5
;!!5 6
set!!7 :
{!!; <
lastNameValue!!= J
=!!K L
value!!M R
;!!R S
OnPropertyChanged!!T e
(!!e f
)!!f g
;!!g h"
raiseCanExecuteChanged!!i 
(	!! Ä
)
!!Ä Å
;
!!Å Ç
}
!!É Ñ
}
!!Ö Ü
public"" 
DateTime"" 
?"" 
DateOfBirth"" $
{""% &
get""' *
=>""+ -
dateOfBirthValue"". >
;""> ?
set""@ C
{""D E
dateOfBirthValue""F V
=""W X
value""Y ^
;""^ _
OnPropertyChanged""` q
(""q r
)""r s
;""s t#
raiseCanExecuteChanged	""u ã
(
""ã å
)
""å ç
;
""ç é
}
""è ê
}
""ë í
public## 
	GenderDto## 
SelectedGender## '
{##( )
get##* -
=>##. 0
selectedGenderValue##1 D
;##D E
set##F I
{##J K
selectedGenderValue##L _
=##` a
value##b g
;##g h
OnPropertyChanged##i z
(##z {
)##{ |
;##| }#
raiseCanExecuteChanged	##~ î
(
##î ï
)
##ï ñ
;
##ñ ó
}
##ò ô
}
##ö õ
public$$  
ObservableCollection$$ #
<$$# $
	GenderDto$$$ -
>$$- .
Genders$$/ 6
{$$7 8
get$$9 <
=>$$= ?
gendersValue$$@ L
;$$L M
set$$N Q
{$$R S
gendersValue$$T `
=$$a b
value$$c h
;$$h i
OnPropertyChanged$$j {
($${ |
)$$| }
;$$} ~
}	$$ Ä
}
$$Å Ç
public%% 
string%% 
AvatarSource%% "
{%%# $
get%%% (
=>%%) +
avatarSourceValue%%, =
;%%= >
set%%? B
{%%C D
avatarSourceValue%%E V
=%%W X
value%%Y ^
;%%^ _
OnPropertyChanged%%` q
(%%q r
)%%r s
;%%s t
}%%u v
}%%w x
public'' 
bool'' *
IsChangePasswordSectionVisible'' 2
{''3 4
get''5 8
=>''9 ;/
#isChangePasswordSectionVisibleValue''< _
;''_ `
set''a d
{''e f0
#isChangePasswordSectionVisibleValue	''g ä
=
''ã å
value
''ç í
;
''í ì
OnPropertyChanged
''î •
(
''• ¶
)
''¶ ß
;
''ß ®
}
''© ™
}
''´ ¨
public(( 
string(( 
CurrentPassword(( %
{((& '
get((( +
=>((, . 
currentPasswordValue((/ C
;((C D
set((E H
{((I J 
currentPasswordValue((K _
=((` a
value((b g
;((g h
OnPropertyChanged((i z
(((z {
)(({ |
;((| }#
raiseCanExecuteChanged	((~ î
(
((î ï
)
((ï ñ
;
((ñ ó
}
((ò ô
}
((ö õ
public)) 
string)) 
NewPassword)) !
{))" #
get))$ '
=>))( *
newPasswordValue))+ ;
;)); <
set))= @
{))A B
newPasswordValue))C S
=))T U
value))V [
;))[ \
OnPropertyChanged))] n
())n o
)))o p
;))p q#
raiseCanExecuteChanged	))r à
(
))à â
)
))â ä
;
))ä ã
}
))å ç
}
))é è
public** 
string** 
ConfirmPassword** %
{**& '
get**( +
=>**, . 
confirmPasswordValue**/ C
;**C D
set**E H
{**I J 
confirmPasswordValue**K _
=**` a
value**b g
;**g h
OnPropertyChanged**i z
(**z {
)**{ |
;**| }#
raiseCanExecuteChanged	**~ î
(
**î ï
)
**ï ñ
;
**ñ ó
}
**ò ô
}
**ö õ
public++ 
bool++ 
IsBusy++ 
{++ 
get++  
=>++! #
isBusyValue++$ /
;++/ 0
private++1 8
set++9 <
{++= >
setBusy++? F
(++F G
value++G L
)++L M
;++M N
}++O P
}++Q R
public-- 
bool-- 
CanSaveChanges-- "
=>--# %
!.. 
IsBusy.. 
&&.. 
!// 
string// 
.// 
IsNullOrWhiteSpace// &
(//& '
	FirstName//' 0
)//0 1
&&//2 4
!00 
string00 
.00 
IsNullOrWhiteSpace00 &
(00& '
LastName00' /
)00/ 0
&&001 3
DateOfBirth11 
.11 
HasValue11  
&&11! #
SelectedGender22 
!=22 
null22 "
;22" #
public44 
bool44 
CanSaveNewPassword44 &
=>44' )
!55 
IsBusy55 
&&55 
!66 
string66 
.66 
IsNullOrWhiteSpace66 &
(66& '
CurrentPassword66' 6
)666 7
&&668 :
!77 
string77 
.77 
IsNullOrWhiteSpace77 &
(77& '
NewPassword77' 2
)772 3
&&774 6
!88 
string88 
.88 
IsNullOrWhiteSpace88 &
(88& '
ConfirmPassword88' 6
)886 7
&&888 :
NewPassword99 
==99 
ConfirmPassword99 *
;99* +
public;; 
ICommand;; 
SaveChangesCommand;; *
{;;+ ,
get;;- 0
;;;0 1
};;2 3
public<< 
ICommand<< 
CancelCommand<< %
{<<& '
get<<( +
;<<+ ,
}<<- .
public== 
ICommand== 
ChangeAvatarCommand== +
{==, -
get==. 1
;==1 2
}==3 4
public>> 
ICommand>> %
ShowChangePasswordCommand>> 1
{>>2 3
get>>4 7
;>>7 8
}>>9 :
public?? 
ICommand?? "
SaveNewPasswordCommand?? .
{??/ 0
get??1 4
;??4 5
}??6 7
public@@ 
ICommand@@ '
CancelChangePasswordCommand@@ 3
{@@4 5
get@@6 9
;@@9 :
}@@; <
publicCC  
EditProfileViewModelCC #
(CC# $
ActionCC$ *
navigateBackCC+ 7
,CC7 8
ActionCC9 ?"
navigateToSelectAvatarCC@ V
)CCV W
{DD 	
thisEE 
.EE 
navigateBackEE 
=EE 
navigateBackEE  ,
;EE, -
thisFF 
.FF "
navigateToSelectAvatarFF '
=FF( )"
navigateToSelectAvatarFF* @
;FF@ A
thisGG 
.GG 
profileClientGG 
=GG  
newGG! $ 
ProfileManagerClientGG% 9
(GG9 :
)GG: ;
;GG; <
CancelCommandII 
=II 
newII 
RelayCommandII  ,
(II, -
pII- .
=>II/ 1
thisII2 6
.II6 7
navigateBackII7 C
?IIC D
.IID E
InvokeIIE K
(IIK L
)IIL M
,IIM N
pIIO P
=>IIQ S
!IIT U
IsBusyIIU [
)II[ \
;II\ ]
SaveChangesCommandJJ 
=JJ  
newJJ! $
RelayCommandJJ% 1
(JJ1 2
asyncJJ2 7
pJJ8 9
=>JJ: <
awaitJJ= B#
saveProfileChangesAsyncJJC Z
(JJZ [
)JJ[ \
,JJ\ ]
pJJ^ _
=>JJ` b
CanSaveChangesJJc q
)JJq r
;JJr s
ChangeAvatarCommandKK 
=KK  !
newKK" %
RelayCommandKK& 2
(KK2 3
pKK3 4
=>KK5 7
thisKK8 <
.KK< ="
navigateToSelectAvatarKK= S
?KKS T
.KKT U
InvokeKKU [
(KK[ \
)KK\ ]
,KK] ^
pKK_ `
=>KKa c
!KKd e
IsBusyKKe k
)KKk l
;KKl m%
ShowChangePasswordCommandLL %
=LL& '
newLL( +
RelayCommandLL, 8
(LL8 9%
executeShowChangePasswordLL9 R
,LLR S
pLLT U
=>LLV X
!LLY Z
IsBusyLLZ `
)LL` a
;LLa b"
SaveNewPasswordCommandMM "
=MM# $
newMM% (
RelayCommandMM) 5
(MM5 6
asyncMM6 ;
pMM< =
=>MM> @
awaitMMA F'
executeSaveNewPasswordAsyncMMG b
(MMb c
)MMc d
,MMd e
pMMf g
=>MMh j
CanSaveNewPasswordMMk }
)MM} ~
;MM~ '
CancelChangePasswordCommandNN '
=NN( )
newNN* -
RelayCommandNN. :
(NN: ;'
executeCancelChangePasswordNN; V
,NNV W
pNNX Y
=>NNZ \
!NN] ^
IsBusyNN^ d
)NNd e
;NNe f
GendersPP 
=PP 
newPP  
ObservableCollectionPP .
<PP. /
	GenderDtoPP/ 8
>PP8 9
(PP9 :
)PP: ;
;PP; <
loadEditableDataQQ 
(QQ 
)QQ 
;QQ 
}RR 	
privateTT 
voidTT %
executeShowChangePasswordTT .
(TT. /
objectTT/ 5
	parameterTT6 ?
)TT? @
{UU 	*
IsChangePasswordSectionVisibleVV *
=VV+ ,
trueVV- 1
;VV1 2
CurrentPasswordWW 
=WW 
stringWW $
.WW$ %
EmptyWW% *
;WW* +
NewPasswordXX 
=XX 
stringXX  
.XX  !
EmptyXX! &
;XX& '
ConfirmPasswordYY 
=YY 
stringYY $
.YY$ %
EmptyYY% *
;YY* +
}ZZ 	
private\\ 
void\\ '
executeCancelChangePassword\\ 0
(\\0 1
object\\1 7
	parameter\\8 A
)\\A B
{]] 	*
IsChangePasswordSectionVisible^^ *
=^^+ ,
false^^- 2
;^^2 3
CurrentPassword__ 
=__ 
string__ $
.__$ %
Empty__% *
;__* +
NewPassword`` 
=`` 
string``  
.``  !
Empty``! &
;``& '
ConfirmPasswordaa 
=aa 
stringaa $
.aa$ %
Emptyaa% *
;aa* +
}bb 	
privatedd 
asyncdd 
Taskdd '
executeSaveNewPasswordAsyncdd 6
(dd6 7
)dd7 8
{ee 	
ifff 
(ff 
NewPasswordff 
!=ff 
ConfirmPasswordff .
)ff. /
{gg 

MessageBoxhh 
.hh 
Showhh 
(hh  
Langhh  $
.hh$ %)
ValidationPasswordsDoNotMatchhh% B
,hhB C
LanghhD H
.hhH I

ErrorTitlehhI S
,hhS T
MessageBoxButtonhhU e
.hhe f
OKhhf h
,hhh i
MessageBoxImagehhj y
.hhy z
Warning	hhz Å
)
hhÅ Ç
;
hhÇ É
returnii 
;ii 
}jj 
ifkk 
(kk 
stringkk 
.kk 
IsNullOrWhiteSpacekk )
(kk) *
CurrentPasswordkk* 9
)kk9 :
||kk; =
stringkk> D
.kkD E
IsNullOrWhiteSpacekkE W
(kkW X
NewPasswordkkX c
)kkc d
)kkd e
{ll 

MessageBoxmm 
.mm 
Showmm 
(mm  
Langmm  $
.mm$ %(
GlobalErrorAllFieldsRequiredmm% A
,mmA B
LangmmC G
.mmG H

ErrorTitlemmH R
,mmR S
MessageBoxButtonmmT d
.mmd e
OKmme g
,mmg h
MessageBoxImagemmi x
.mmx y
Warning	mmy Ä
)
mmÄ Å
;
mmÅ Ç
returnnn 
;nn 
}oo 
setBusyqq 
(qq 
trueqq 
)qq 
;qq 
tryrr 
{ss 
vartt 
resulttt 
=tt 
awaittt "
profileClienttt# 0
.tt0 1
changePasswordAsynctt1 D
(ttD E
SessionServicettE S
.ttS T
UsernamettT \
,tt\ ]
CurrentPasswordtt^ m
,ttm n
NewPasswordtto z
)ttz {
;tt{ |
ifvv 
(vv 
resultvv 
.vv 
successvv "
)vv" #
{ww 

MessageBoxxx 
.xx 
Showxx #
(xx# $
resultxx$ *
.xx* +
messagexx+ 2
,xx2 3
Langxx4 8
.xx8 9
InfoMsgTitleSuccessxx9 L
,xxL M
MessageBoxButtonxxN ^
.xx^ _
OKxx_ a
,xxa b
MessageBoxImagexxc r
.xxr s
Informationxxs ~
)xx~ 
;	xx Ä'
executeCancelChangePasswordyy /
(yy/ 0
nullyy0 4
)yy4 5
;yy5 6
}zz 
else{{ 
{|| 

MessageBox}} 
.}} 
Show}} #
(}}# $
result}}$ *
.}}* +
message}}+ 2
,}}2 3
Lang}}4 8
.}}8 9

ErrorTitle}}9 C
,}}C D
MessageBoxButton}}E U
.}}U V
OK}}V X
,}}X Y
MessageBoxImage}}Z i
.}}i j
Error}}j o
)}}o p
;}}p q
}~~ 
} 
catch
ÄÄ 
(
ÄÄ 
	Exception
ÄÄ 
ex
ÄÄ 
)
ÄÄ  
{
ÅÅ 
handleError
ÇÇ 
(
ÇÇ 
$str
ÇÇ 5
,
ÇÇ5 6
ex
ÇÇ7 9
)
ÇÇ9 :
;
ÇÇ: ;
}
ÉÉ 
finally
ÑÑ 
{
ÖÖ 
setBusy
ÜÜ 
(
ÜÜ 
false
ÜÜ 
)
ÜÜ 
;
ÜÜ 
}
áá 
}
àà 	
private
ãã 
async
ãã 
void
ãã 
loadEditableData
ãã +
(
ãã+ ,
)
ãã, -
{
åå 	
setBusy
çç 
(
çç 
true
çç 
)
çç 
;
çç 
try
éé 
{
èè 
AvatarSource
êê 
=
êê 
SessionService
êê -
.
êê- .

AvatarPath
êê. 8
??
êê9 ;
$str
êê< i
;
êêi j
var
ëë 
profileData
ëë 
=
ëë  !
await
ëë" '
profileClient
ëë( 5
.
ëë5 6*
getPlayerProfileForEditAsync
ëë6 R
(
ëëR S
SessionService
ëëS a
.
ëëa b
Username
ëëb j
)
ëëj k
;
ëëk l
if
íí 
(
íí 
profileData
íí 
!=
íí  "
null
íí# '
)
íí' (
{
ìì 
	FirstName
îî 
=
îî 
profileData
îî  +
.
îî+ ,
	firstName
îî, 5
;
îî5 6
LastName
ïï 
=
ïï 
profileData
ïï *
.
ïï* +
lastName
ïï+ 3
;
ïï3 4
DateOfBirth
ññ 
=
ññ  !
profileData
ññ" -
.
ññ- .
dateOfBirth
ññ. 9
;
ññ9 :
Genders
óó 
.
óó 
Clear
óó !
(
óó! "
)
óó" #
;
óó# $
if
òò 
(
òò 
profileData
òò #
.
òò# $
availableGenders
òò$ 4
!=
òò5 7
null
òò8 <
)
òò< =
{
ôô 
foreach
öö 
(
öö  !
var
öö! $
gender
öö% +
in
öö, .
profileData
öö/ :
.
öö: ;
availableGenders
öö; K
)
ööK L
{
õõ 
Genders
úú #
.
úú# $
Add
úú$ '
(
úú' (
gender
úú( .
)
úú. /
;
úú/ 0
}
ùù 
}
ûû 
SelectedGender
üü "
=
üü# $
Genders
üü% ,
.
üü, -
FirstOrDefault
üü- ;
(
üü; <
g
üü< =
=>
üü> @
g
üüA B
.
üüB C
idGender
üüC K
==
üüL N
profileData
üüO Z
.
üüZ [
idGender
üü[ c
)
üüc d
;
üüd e
}
†† 
else
°° 
{
¢¢ 

MessageBox
££ 
.
££ 
Show
££ #
(
££# $
Lang
££$ (
.
££( )&
ErrorFailedToLoadProfile
££) A
,
££A B
Lang
££C G
.
££G H

ErrorTitle
££H R
,
££R S
MessageBoxButton
££T d
.
££d e
OK
££e g
,
££g h
MessageBoxImage
££i x
.
££x y
Warning££y Ä
)££Ä Å
;££Å Ç
}
§§ $
raiseCanExecuteChanged
•• &
(
••& '
)
••' (
;
••( )
}
¶¶ 
catch
ßß 
(
ßß 
	Exception
ßß 
ex
ßß 
)
ßß  
{
®® 
handleError
©© 
(
©© 
Lang
©©  
.
©©  !&
ErrorFailedToLoadProfile
©©! 9
,
©©9 :
ex
©©; =
)
©©= >
;
©©> ?
navigateBack
™™ 
?
™™ 
.
™™ 
Invoke
™™ $
(
™™$ %
)
™™% &
;
™™& '
}
´´ 
finally
¨¨ 
{
≠≠ 
setBusy
ÆÆ 
(
ÆÆ 
false
ÆÆ 
)
ÆÆ 
;
ÆÆ 
}
ØØ 
}
∞∞ 	
private
≤≤ 
async
≤≤ 
Task
≤≤ %
saveProfileChangesAsync
≤≤ 2
(
≤≤2 3
)
≤≤3 4
{
≥≥ 	
if
¥¥ 
(
¥¥ 
string
¥¥ 
.
¥¥  
IsNullOrWhiteSpace
¥¥ )
(
¥¥) *
	FirstName
¥¥* 3
)
¥¥3 4
||
¥¥5 7
string
¥¥8 >
.
¥¥> ? 
IsNullOrWhiteSpace
¥¥? Q
(
¥¥Q R
LastName
¥¥R Z
)
¥¥Z [
||
¥¥\ ^
!
¥¥_ `
DateOfBirth
¥¥` k
.
¥¥k l
HasValue
¥¥l t
||
¥¥u w
SelectedGender¥¥x Ü
==¥¥á â
null¥¥ä é
)¥¥é è
{
µµ 

MessageBox
∂∂ 
.
∂∂ 
Show
∂∂ 
(
∂∂  
Lang
∂∂  $
.
∂∂$ %*
GlobalErrorAllFieldsRequired
∂∂% A
,
∂∂A B
Lang
∂∂C G
.
∂∂G H

ErrorTitle
∂∂H R
,
∂∂R S
MessageBoxButton
∂∂T d
.
∂∂d e
OK
∂∂e g
,
∂∂g h
MessageBoxImage
∂∂i x
.
∂∂x y
Warning∂∂y Ä
)∂∂Ä Å
;∂∂Å Ç
return
∑∑ 
;
∑∑ 
}
∏∏ 
var
∫∫ 
updatedProfile
∫∫ 
=
∫∫  
new
∫∫! $#
UserProfileForEditDto
∫∫% :
{
ªª 
	firstName
ºº 
=
ºº 
this
ºº  
.
ºº  !
	FirstName
ºº! *
,
ºº* +
lastName
ΩΩ 
=
ΩΩ 
this
ΩΩ 
.
ΩΩ  
LastName
ΩΩ  (
,
ΩΩ( )
dateOfBirth
ææ 
=
ææ 
this
ææ "
.
ææ" #
DateOfBirth
ææ# .
,
ææ. /
idGender
øø 
=
øø 
this
øø 
.
øø  
SelectedGender
øø  .
.
øø. /
idGender
øø/ 7
,
øø7 8
availableGenders
¿¿  
=
¿¿! "
null
¿¿# '
}
¡¡ 
;
¡¡ 
setBusy
√√ 
(
√√ 
true
√√ 
)
√√ 
;
√√ 
try
ƒƒ 
{
≈≈ 
var
∆∆ 
result
∆∆ 
=
∆∆ 
await
∆∆ "
profileClient
∆∆# 0
.
∆∆0 1 
updateProfileAsync
∆∆1 C
(
∆∆C D
SessionService
∆∆D R
.
∆∆R S
Username
∆∆S [
,
∆∆[ \
updatedProfile
∆∆] k
)
∆∆k l
;
∆∆l m
if
»» 
(
»» 
result
»» 
.
»» 
success
»» "
)
»»" #
{
…… 

MessageBox
   
.
   
Show
   #
(
  # $
$str
  $ C
,
  C D
Lang
  E I
.
  I J!
InfoMsgTitleSuccess
  J ]
,
  ] ^
MessageBoxButton
  _ o
.
  o p
OK
  p r
,
  r s
MessageBoxImage  t É
.  É Ñ
Information  Ñ è
)  è ê
;  ê ë
navigateBack
ÀÀ  
?
ÀÀ  !
.
ÀÀ! "
Invoke
ÀÀ" (
(
ÀÀ( )
)
ÀÀ) *
;
ÀÀ* +
}
ÃÃ 
else
ÕÕ 
{
ŒŒ 

MessageBox
œœ 
.
œœ 
Show
œœ #
(
œœ# $
result
œœ$ *
.
œœ* +
message
œœ+ 2
,
œœ2 3
Lang
œœ4 8
.
œœ8 9

ErrorTitle
œœ9 C
,
œœC D
MessageBoxButton
œœE U
.
œœU V
OK
œœV X
,
œœX Y
MessageBoxImage
œœZ i
.
œœi j
Error
œœj o
)
œœo p
;
œœp q
}
–– 
}
—— 
catch
““ 
(
““ 
	Exception
““ 
ex
““ 
)
““  
{
”” 
handleError
‘‘ 
(
‘‘ 
$str
‘‘ 4
,
‘‘4 5
ex
‘‘6 8
)
‘‘8 9
;
‘‘9 :
}
’’ 
finally
÷÷ 
{
◊◊ 
setBusy
ÿÿ 
(
ÿÿ 
false
ÿÿ 
)
ÿÿ 
;
ÿÿ 
}
ŸŸ 
}
⁄⁄ 	
public
‹‹ 
void
‹‹ 
RefreshAvatar
‹‹ !
(
‹‹! "
)
‹‹" #
{
›› 	
AvatarSource
ﬁﬁ 
=
ﬁﬁ 
SessionService
ﬁﬁ )
.
ﬁﬁ) *

AvatarPath
ﬁﬁ* 4
??
ﬁﬁ5 7
$str
ﬁﬁ8 e
;
ﬁﬁe f
}
ﬂﬂ 	
private
·· 
void
·· 
setBusy
·· 
(
·· 
bool
·· !
value
··" '
)
··' (
{
‚‚ 	
isBusyValue
„„ 
=
„„ 
value
„„ 
;
„„  
OnPropertyChanged
‰‰ 
(
‰‰ 
nameof
‰‰ $
(
‰‰$ %
IsBusy
‰‰% +
)
‰‰+ ,
)
‰‰, -
;
‰‰- .$
raiseCanExecuteChanged
ÂÂ "
(
ÂÂ" #
)
ÂÂ# $
;
ÂÂ$ %
}
ÊÊ 	
private
ËË 
void
ËË $
raiseCanExecuteChanged
ËË +
(
ËË+ ,
)
ËË, -
{
ÈÈ 	
OnPropertyChanged
ÍÍ 
(
ÍÍ 
nameof
ÍÍ $
(
ÍÍ$ %
CanSaveChanges
ÍÍ% 3
)
ÍÍ3 4
)
ÍÍ4 5
;
ÍÍ5 6
OnPropertyChanged
ÎÎ 
(
ÎÎ 
nameof
ÎÎ $
(
ÎÎ$ % 
CanSaveNewPassword
ÎÎ% 7
)
ÎÎ7 8
)
ÎÎ8 9
;
ÎÎ9 :
Application
ÏÏ 
.
ÏÏ 
Current
ÏÏ 
?
ÏÏ  
.
ÏÏ  !

Dispatcher
ÏÏ! +
?
ÏÏ+ ,
.
ÏÏ, -
Invoke
ÏÏ- 3
(
ÏÏ3 4
(
ÏÏ4 5
)
ÏÏ5 6
=>
ÏÏ7 9
CommandManager
ÏÏ: H
.
ÏÏH I(
InvalidateRequerySuggested
ÏÏI c
(
ÏÏc d
)
ÏÏd e
)
ÏÏe f
;
ÏÏf g
}
ÌÌ 	
private
 
void
 
handleError
  
(
  !
string
! '
message
( /
,
/ 0
	Exception
1 :
ex
; =
)
= >
{
ÒÒ 	
string
ÚÚ 
errorDetails
ÚÚ 
=
ÚÚ  !
ex
ÚÚ" $
?
ÚÚ$ %
.
ÚÚ% &
Message
ÚÚ& -
??
ÚÚ. 0
$str
ÚÚ1 G
;
ÚÚG H
Console
ÛÛ 
.
ÛÛ 
	WriteLine
ÛÛ 
(
ÛÛ 
$"
ÛÛ  
$str
ÛÛ  $
{
ÛÛ$ %
message
ÛÛ% ,
}
ÛÛ, -
$str
ÛÛ- /
{
ÛÛ/ 0
errorDetails
ÛÛ0 <
}
ÛÛ< =
"
ÛÛ= >
)
ÛÛ> ?
;
ÛÛ? @

MessageBox
ÙÙ 
.
ÙÙ 
Show
ÙÙ 
(
ÙÙ 
$"
ÙÙ 
{
ÙÙ 
message
ÙÙ &
}
ÙÙ& '
$str
ÙÙ' *
{
ÙÙ* +
errorDetails
ÙÙ+ 7
}
ÙÙ7 8
"
ÙÙ8 9
,
ÙÙ9 :
Lang
ÙÙ; ?
.
ÙÙ? @

ErrorTitle
ÙÙ@ J
,
ÙÙJ K
MessageBoxButton
ÙÙL \
.
ÙÙ\ ]
OK
ÙÙ] _
,
ÙÙ_ `
MessageBoxImage
ÙÙa p
.
ÙÙp q
Error
ÙÙq v
)
ÙÙv w
;
ÙÙw x
}
ıı 	
}
ˆˆ 
}˜˜ •ó
êC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\ViewModel\Game\LobbyViewModel.cs
	namespace 	
MindWeaveClient
 
. 
	ViewModel #
.# $
Game$ (
{ 
public 
class 
LobbyViewModel 
: 
BaseViewModel ,
{ 
public 
bool 
IsGuestUser 
=> 
SessionService *
.* +
IsGuest+ 2
;2 3
private $
MatchmakingManagerClient	 !
matchmakingProxy" 2
=>3 5+
MatchmakingServiceClientManager6 U
.U V
instanceV ^
.^ _
proxy_ d
;d e
private &
MatchmakingCallbackHandler	 #&
matchmakingCallbackHandler$ >
=>? A+
MatchmakingServiceClientManagerB a
.a b
instanceb j
.j k
callbackHandlerk z
;z {
private 
SocialManagerClient	 
socialProxy (
=>) +&
SocialServiceClientManager, F
.F G
instanceG O
.O P
proxyP U
;U V
private   
ChatManagerClient  	 
	chatProxy   $
=>  % '$
ChatServiceClientManager  ( @
.  @ A
instance  A I
.  I J
proxy  J O
;  O P
private!! 
ChatCallbackHandler!!	 
chatCallbackHandler!! 0
=>!!1 3$
ChatServiceClientManager!!4 L
.!!L M
instance!!M U
.!!U V
callbackHandler!!V e
;!!e f
private$$ 
readonly$$	 
Action$$ 
<$$ 
Page$$ 
>$$ 

navigateTo$$ )
;$$) *
private%% 
readonly%%	 
Action%% 
navigateBack%% %
;%%% &
private'' 
string''	 
lobbyCodeValue'' 
;'' 
public(( 
string(( 
	LobbyCode(( 
{(( 
get(( 
=>(( !
lobbyCodeValue((" 0
;((0 1
set((2 5
{((6 7
lobbyCodeValue((8 F
=((G H
value((I N
;((N O
OnPropertyChanged((P a
(((a b
)((b c
;((c d
}((e f
}((g h
private)) 
string))	 
hostUsernameValue)) !
;))! "
public** 
string** 
HostUsername** 
{** 
get** !
=>**" $
hostUsernameValue**% 6
;**6 7
set**8 ;
{**< =
hostUsernameValue**> O
=**P Q
value**R W
;**W X
OnPropertyChanged**Y j
(**j k
)**k l
;**l m
OnPropertyChanged**n 
(	** Ä
nameof
**Ä Ü
(
**Ü á
IsHost
**á ç
)
**ç é
)
**é è
;
**è ê
}
**ë í
}
**ì î
public++  
ObservableCollection++ 
<++ 
string++ #
>++# $
Players++% ,
{++- .
get++/ 2
;++2 3
}++4 5
=++6 7
new++8 ; 
ObservableCollection++< P
<++P Q
string++Q W
>++W X
(++X Y
)++Y Z
;++Z [
private,, 
LobbySettingsDto,,	  
currentSettingsValue,, .
;,,. /
public-- 
LobbySettingsDto-- 
CurrentSettings-- (
{--) *
get--+ .
=>--/ 1 
currentSettingsValue--2 F
;--F G
set--H K
{--L M 
currentSettingsValue--N b
=--c d
value--e j
;--j k
OnPropertyChanged--l }
(--} ~
)--~ 
;	-- Ä
}
--Å Ç
}
--É Ñ
public.. 
bool.. 
IsHost.. 
=>.. 
hostUsernameValue.. (
==..) +
SessionService.., :
...: ;
Username..; C
;..C D
private// 
bool//	 
isBusyValue// 
;// 
public00 
bool00 
IsBusy00 
{00 
get00 
=>00 
isBusyValue00 (
;00( )
set00* -
{00. /
isBusyValue000 ;
=00< =
value00> C
;00C D
OnPropertyChanged00E V
(00V W
)00W X
;00X Y,
 RaiseCanExecuteChangedOnCommands00Z z
(00z {
)00{ |
;00| }
}00~ 
}
00Ä Å
public11  
ObservableCollection11 
<11 
FriendDtoDisplay11 -
>11- .
OnlineFriends11/ <
{11= >
get11? B
;11B C
}11D E
=11F G
new11H K 
ObservableCollection11L `
<11` a
FriendDtoDisplay11a q
>11q r
(11r s
)11s t
;11t u
public33  
ObservableCollection33 
<33 '
ChatMessageDisplayViewModel33 8
>338 9
ChatMessages33: F
{33G H
get33I L
;33L M
}33N O
=33P Q
new33R U 
ObservableCollection33V j
<33j k(
ChatMessageDisplayViewModel	33k Ü
>
33Ü á
(
33á à
)
33à â
;
33â ä
private44 
string44	 #
currentChatMessageValue44 '
=44( )
string44* 0
.440 1
Empty441 6
;446 7
public55 
string55 
CurrentChatMessage55 !
{66 
get77 
=>77 #
currentChatMessageValue77 
;77  
set88 
{88 #
currentChatMessageValue88 
=88  
value88! &
;88& '
OnPropertyChanged88( 9
(889 :
)88: ;
;88; <
(88= >
(88> ?
RelayCommand88? K
)88K L
SendMessageCommand88L ^
)88^ _
.88_ `"
RaiseCanExecuteChanged88` v
(88v w
)88w x
;88x y
}88z {
}99 
public:: 
ICommand:: 
LeaveLobbyCommand:: "
{::# $
get::% (
;::( )
}::* +
public;; 
ICommand;; 
StartGameCommand;; !
{;;" #
get;;$ '
;;;' (
};;) *
public<< 
ICommand<< 
InviteFriendCommand<< $
{<<% &
get<<' *
;<<* +
}<<, -
public== 
ICommand== 
KickPlayerCommand== "
{==# $
get==% (
;==( )
}==* +
public>> 
ICommand>> 
UploadImageCommand>> #
{>>$ %
get>>& )
;>>) *
}>>+ ,
public?? 
ICommand?? !
ChangeSettingsCommand?? &
{??' (
get??) ,
;??, -
}??. /
public@@ 
ICommand@@ !
RefreshFriendsCommand@@ &
{@@' (
get@@) ,
;@@, -
}@@. /
publicAA 
ICommandAA 
SendMessageCommandAA #
{AA$ %
getAA& )
;AA) *
}AA+ ,
publicBB 
ICommandBB 
InviteGuestCommandBB #
{BB$ %
getBB& )
;BB) *
}BB+ ,
publicDD 
LobbyViewModelDD 
(DD 
LobbyStateDtoDD $
initialStateDD% 1
,DD1 2
ActionDD3 9
<DD9 :
PageDD: >
>DD> ?
navigateToActionDD@ P
,DDP Q
ActionDDR X
navigateBackActionDDY k
)DDk l
{EE 
thisFF 
.FF 

navigateToFF 
=FF 
navigateToActionFF #
;FF# $
thisGG 
.GG 
navigateBackGG 
=GG 
navigateBackActionGG '
;GG' (
LeaveLobbyCommandII 
=II 
newII 
RelayCommandII %
(II% &
executeLeaveLobbyII& 7
,II7 8
paramII9 >
=>II? A
!IIB C
IsBusyIIC I
)III J
;IIJ K
StartGameCommandJJ 
=JJ 
newJJ 
RelayCommandJJ $
(JJ$ %
executeStartGameJJ% 5
,JJ5 6
paramJJ7 <
=>JJ= ?
IsHostJJ@ F
&&JJG I
!JJJ K
IsBusyJJK Q
&&JJR T
!JJU V
IsGuestUserJJV a
)JJa b
;JJb c
InviteFriendCommandKK 
=KK 
newKK 
RelayCommandKK '
(KK' (
executeInviteFriendKK( ;
,KK; <
paramKK= B
=>KKC E
IsHostKKF L
&&KKM O
!KKP Q
IsBusyKKQ W
&&KKX Z
!KK[ \
IsGuestUserKK\ g
&&KKh j
paramKKk p
isKKq s
FriendDtoDisplay	KKt Ñ
friend
KKÖ ã
&&
KKå é
friend
KKè ï
.
KKï ñ
IsOnline
KKñ û
&&
KKü °
!
KK¢ £
Players
KK£ ™
.
KK™ ´
Contains
KK´ ≥
(
KK≥ ¥
friend
KK¥ ∫
.
KK∫ ª
Username
KKª √
)
KK√ ƒ
)
KKƒ ≈
;
KK≈ ∆
KickPlayerCommandLL 
=LL 
newLL 
RelayCommandLL %
(LL% &
executeKickPlayerLL& 7
,LL7 8
pLL9 :
=>LL; =
IsHostLL> D
&&LLE G
!LLH I
IsBusyLLI O
&&LLP R
!LLS T
IsGuestUserLLT _
&&LL` b
pLLc d
isLLe g
stringLLh n
targetLLo u
&&LLv x
targetLLy 
!=
LLÄ Ç
HostUsername
LLÉ è
)
LLè ê
;
LLê ë
UploadImageCommandMM 
=MM 
newMM 
RelayCommandMM &
(MM& '
pMM' (
=>MM) +

MessageBoxMM, 6
.MM6 7
ShowMM7 ;
(MM; <
$strMM< N
)MMN O
,MMO P
paramMMQ V
=>MMW Y
IsHostMMZ `
&&MMa c
!MMd e
IsBusyMMe k
&&MMl n
!MMo p
IsGuestUserMMp {
)MM{ |
;MM| }!
ChangeSettingsCommandNN 
=NN 
newNN 
RelayCommandNN )
(NN) *
pNN* +
=>NN, .

MessageBoxNN/ 9
.NN9 :
ShowNN: >
(NN> ?
$strNN? T
)NNT U
,NNU V
paramNNW \
=>NN] _
IsHostNN` f
&&NNg i
!NNj k
IsBusyNNk q
&&NNr t
!NNu v
IsGuestUser	NNv Å
)
NNÅ Ç
;
NNÇ É!
RefreshFriendsCommandOO 
=OO 
newOO 
RelayCommandOO )
(OO) *
asyncOO* /
pOO0 1
=>OO2 4
awaitOO5 :"
loadOnlineFriendsAsyncOO; Q
(OOQ R
)OOR S
,OOS T
pOOU V
=>OOW Y
IsHostOOZ `
&&OOa c
!OOd e
IsBusyOOe k
&&OOl n
!OOo p
IsGuestUserOOp {
)OO{ |
;OO| }
SendMessageCommandPP 
=PP 
newPP 
RelayCommandPP &
(PP& '
executeSendMessagePP' 9
,PP9 :!
canExecuteSendMessagePP; P
)PPP Q
;PPQ R
InviteGuestCommandQQ 
=QQ 
newQQ 
RelayCommandQQ &
(QQ& '
asyncQQ' ,
pQQ- .
=>QQ/ 1
awaitQQ2 7#
executeInviteGuestAsyncQQ8 O
(QQO P
)QQP Q
,QQQ R
pQQS T
=>QQU W
IsHostQQX ^
&&QQ_ a
!QQb c
IsBusyQQc i
&&QQj l
!QQm n
IsGuestUserQQn y
)QQy z
;QQz { 
subscribeToCallbacksSS 
(SS 
)SS 
;SS 
ifUU 
(UU 
initialStateUU 
!=UU 
nullUU 
)UU 
{UU 
connectToChatVV 
(VV 
initialStateVV 
.VV 
lobbyIdVV #
)VV# $
;VV$ %
updateStateWW 
(WW 
initialStateWW 
)WW 
;WW 
}XX 
elseYY 
{YY 
	LobbyCodeYY 
=YY 
$strYY  
;YY  !
HostUsernameYY" .
=YY/ 0
$strYY1 =
;YY= >
}YY? @
if[[ 
([[ 
IsHost[[ 
&&[[ 
![[ 
IsGuestUser[[ 
)[[ 
{[[ !
RefreshFriendsCommand[[ 4
.[[4 5
Execute[[5 <
([[< =
null[[= A
)[[A B
;[[B C
}[[D E
OnPropertyChanged\\ 
(\\ 
nameof\\ 
(\\ 
IsGuestUser\\ %
)\\% &
)\\& '
;\\' (,
 RaiseCanExecuteChangedOnCommands]] !
(]]! "
)]]" #
;]]# $
Debug^^ 
.^^ 
	WriteLine^^ 
(^^ 
$"^^ 
$str^^ C
{^^C D
initialState^^D P
==^^Q S
null^^T X
}^^X Y
"^^Y Z
)^^Z [
;^^[ \
}`` 
privatebb 
asyncbb	 
Taskbb #
executeInviteGuestAsyncbb +
(bb+ ,
)bb, -
{cc 
ifdd 
(dd 
!dd 
IsHostdd 
||dd 
IsGuestUserdd 
||dd 
IsBusydd %
)dd% &
returndd' -
;dd- .
varff 
dialogff 
=ff 
newff 
GuestInputDialogff "
(ff" #
)ff# $
;ff$ %
dialoggg 
.gg 
Ownergg 
=gg 
Applicationgg 
.gg 
Currentgg #
.gg# $
Windowsgg$ +
.gg+ ,
OfTypegg, 2
<gg2 3
Windowgg3 9
>gg9 :
(gg: ;
)gg; <
.gg< =
FirstOrDefaultgg= K
(ggK L
wggL M
=>ggN P
wggQ R
.ggR S
IsActiveggS [
)gg[ \
;gg\ ]
boolii 
?ii 
dialogResultii 
=ii 
dialogii 
.ii 

ShowDialogii '
(ii' (
)ii( )
;ii) *
ifkk 
(kk 
dialogResultkk 
!=kk 
truekk 
)kk 
{ll 
returnmm 
;mm 
}nn 
stringpp 

guestEmailpp 
=pp 
dialogpp 
.pp 

GuestEmailpp &
;pp& '
ifrr 
(rr 
stringrr 
.rr 
IsNullOrWhiteSpacerr 
(rr 

guestEmailrr )
)rr) *
)rr* +
{ss 
returntt 
;tt 
}uu 
ifww 
(ww 
!ww 
Regexww 
.ww 
IsMatchww 
(ww 

guestEmailww 
,ww 
$strww  =
)ww= >
)ww> ?
{xx 

MessageBoxyy 
.yy 
Showyy 
(yy 
Langyy 
.yy )
GlobalErrorInvalidEmailFormatyy 3
,yy3 4
Langyy5 9
.yy9 :

ErrorTitleyy: D
,yyD E
MessageBoxButtonyyF V
.yyV W
OKyyW Y
,yyY Z
MessageBoxImageyy[ j
.yyj k
Warningyyk r
)yyr s
;yys t
returnzz 
;zz 
}{{ 
if}} 
(}} 
!}} +
MatchmakingServiceClientManager}} %
.}}% &
instance}}& .
.}}. /
EnsureConnected}}/ >
(}}> ?
)}}? @
)}}@ A
{~~ 

MessageBox 
. 
Show 
( 
Lang 
. $
CannotConnectMatchmaking .
,. /
Lang0 4
.4 5

ErrorTitle5 ?
,? @
MessageBoxButtonA Q
.Q R
OKR T
,T U
MessageBoxImageV e
.e f
Warningf m
)m n
;n o
return
ÄÄ 
;
ÄÄ 
}
ÅÅ 
setBusy
ÉÉ 
(
ÉÉ 	
true
ÉÉ	 
)
ÉÉ 
;
ÉÉ 
var
ÖÖ 
invitationData
ÖÖ 
=
ÖÖ 
new
ÖÖ  
GuestInvitationDto
ÖÖ ,
{
ÜÜ 
inviterUsername
áá 
=
áá 
SessionService
áá !
.
áá! "
Username
áá" *
,
áá* +

guestEmail
àà 
=
àà 

guestEmail
àà 
.
àà 
Trim
àà 
(
àà 
)
àà 
,
àà  
	lobbyCode
ââ 

=
ââ 
this
ââ 
.
ââ 
	LobbyCode
ââ 
}
ää 
;
ää 
try
åå 
{
çç 
await
éé 
matchmakingProxy
éé 
.
éé %
inviteGuestByEmailAsync
éé /
(
éé/ 0
invitationData
éé0 >
)
éé> ?
;
éé? @

MessageBox
èè 
.
èè 
Show
èè 
(
èè 
$"
èè 
$str
èè &
{
èè& '

guestEmail
èè' 1
}
èè1 2
$str
èè2 3
"
èè3 4
,
èè4 5
Lang
èè6 :
.
èè: ;!
InfoMsgTitleSuccess
èè; N
,
èèN O
MessageBoxButton
èèP `
.
èè` a
OK
èèa c
,
èèc d
MessageBoxImage
èèe t
.
èèt u
Informationèèu Ä
)èèÄ Å
;èèÅ Ç
}
êê 
catch
ëë 
(
ëë 
	Exception
ëë 
ex
ëë 
)
ëë 
{
íí 
handleError
ìì 
(
ìì 
Lang
ìì 
.
ìì )
ErrorSendingGuestInvitation
ìì -
,
ìì- .
ex
ìì/ 1
)
ìì1 2
;
ìì2 3-
MatchmakingServiceClientManager
îî  
.
îî  !
instance
îî! )
.
îî) *

Disconnect
îî* 4
(
îî4 5
)
îî5 6
;
îî6 7
}
ïï 
finally
ññ 
{
óó 
setBusy
òò 
(
òò 	
false
òò	 
)
òò 
;
òò 
}
ôô 
}
öö 
private
úú 
void
úú	 
connectToChat
úú 
(
úú 
string
úú "
lobbyIdToConnect
úú# 3
)
úú3 4
{
ùù 
Debug
ûû 
.
ûû 
	WriteLine
ûû 
(
ûû 
$"
ûû 
$str
ûû ;
{
ûû; <
lobbyIdToConnect
ûû< L
}
ûûL M
$str
ûûM P
"
ûûP Q
)
ûûQ R
;
ûûR S
if
üü 
(
üü &
ChatServiceClientManager
üü 
.
üü 
instance
üü &
.
üü& '
Connect
üü' .
(
üü. /
SessionService
üü/ =
.
üü= >
Username
üü> F
,
üüF G
lobbyIdToConnect
üüH X
)
üüX Y
)
üüY Z
{
†† 
Debug
°° 
.
°° 
	WriteLine
°° 
(
°° 
$"
°° 
$str
°° 9
{
°°9 :
lobbyIdToConnect
°°: J
}
°°J K
$str
°°K [
"
°°[ \
)
°°\ ]
;
°°] ^&
subscribeToChatCallbacks
¢¢ 
(
¢¢ 
)
¢¢ 
;
¢¢ 
}
££ 
else
§§ 
{
•• 
Debug
¶¶ 
.
¶¶ 
	WriteLine
¶¶ 
(
¶¶ 
$"
¶¶ 
$str
¶¶ ?
{
¶¶? @
lobbyIdToConnect
¶¶@ P
}
¶¶P Q
$str
¶¶Q R
"
¶¶R S
)
¶¶S T
;
¶¶T U

MessageBox
ßß 
.
ßß 
Show
ßß 
(
ßß 
$str
ßß 3
,
ßß3 4
Lang
ßß5 9
.
ßß9 :

ErrorTitle
ßß: D
,
ßßD E
MessageBoxButton
ßßF V
.
ßßV W
OK
ßßW Y
,
ßßY Z
MessageBoxImage
ßß[ j
.
ßßj k
Warning
ßßk r
)
ßßr s
;
ßßs t
}
®® 
}
©© 
private
´´ 
void
´´	  
disconnectFromChat
´´  
(
´´  !
)
´´! "
{
¨¨ 
Debug
≠≠ 
.
≠≠ 
	WriteLine
≠≠ 
(
≠≠ 
$str
≠≠ 5
)
≠≠5 6
;
≠≠6 7*
unsubscribeFromChatCallbacks
ÆÆ 
(
ÆÆ 
)
ÆÆ 
;
ÆÆ  &
ChatServiceClientManager
ØØ 
.
ØØ 
instance
ØØ "
.
ØØ" #

Disconnect
ØØ# -
(
ØØ- .
)
ØØ. /
;
ØØ/ 0
}
∞∞ 
private
≤≤ 
void
≤≤	 &
subscribeToChatCallbacks
≤≤ &
(
≤≤& '
)
≤≤' (
{
≥≥ 
if
¥¥ 
(
¥¥ !
chatCallbackHandler
¥¥ 
!=
¥¥ 
null
¥¥  
)
¥¥  !
{
µµ !
chatCallbackHandler
∂∂ 
.
∂∂ "
LobbyMessageReceived
∂∂ )
-=
∂∂* ,(
handleLobbyMessageReceived
∂∂- G
;
∂∂G H!
chatCallbackHandler
∑∑ 
.
∑∑ "
LobbyMessageReceived
∑∑ )
+=
∑∑* ,(
handleLobbyMessageReceived
∑∑- G
;
∑∑G H
Debug
∏∏ 
.
∏∏ 
	WriteLine
∏∏ 
(
∏∏ 
$str
∏∏ 6
)
∏∏6 7
;
∏∏7 8
}
ππ 
else
∫∫ 
{
ªª 
Debug
ºº 
.
ºº 
	WriteLine
ºº 
(
ºº 
$str
ºº N
)
ººN O
;
ººO P
}
ΩΩ 
}
ææ 
private
¿¿ 
void
¿¿	 *
unsubscribeFromChatCallbacks
¿¿ *
(
¿¿* +
)
¿¿+ ,
{
¡¡ 
if
¬¬ 
(
¬¬ !
chatCallbackHandler
¬¬ 
!=
¬¬ 
null
¬¬  
)
¬¬  !
{
√√ !
chatCallbackHandler
ƒƒ 
.
ƒƒ "
LobbyMessageReceived
ƒƒ )
-=
ƒƒ* ,(
handleLobbyMessageReceived
ƒƒ- G
;
ƒƒG H
Debug
≈≈ 
.
≈≈ 
	WriteLine
≈≈ 
(
≈≈ 
$str
≈≈ :
)
≈≈: ;
;
≈≈; <
}
∆∆ 
}
«« 
private
…… 
void
……	 (
handleLobbyMessageReceived
…… (
(
……( )
ChatMessageDto
……) 7

messageDto
……8 B
)
……B C
{
   
Application
ÀÀ 
.
ÀÀ 
Current
ÀÀ 
?
ÀÀ 
.
ÀÀ 

Dispatcher
ÀÀ  
?
ÀÀ  !
.
ÀÀ! "
Invoke
ÀÀ" (
(
ÀÀ( )
(
ÀÀ) *
)
ÀÀ* +
=>
ÀÀ, .
{
ÃÃ 
ChatMessages
ÕÕ 
.
ÕÕ 
Add
ÕÕ 
(
ÕÕ 
new
ÕÕ )
ChatMessageDisplayViewModel
ÕÕ 1
(
ÕÕ1 2

messageDto
ÕÕ2 <
)
ÕÕ< =
)
ÕÕ= >
;
ÕÕ> ?
Debug
ŒŒ 
.
ŒŒ 
	WriteLine
ŒŒ 
(
ŒŒ 
$"
ŒŒ 
$str
ŒŒ &
{
ŒŒ& '

messageDto
ŒŒ' 1
.
ŒŒ1 2
senderUsername
ŒŒ2 @
}
ŒŒ@ A
$str
ŒŒA S
"
ŒŒS T
)
ŒŒT U
;
ŒŒU V
}
œœ 
)
œœ 
;
œœ 
}
–– 
private
”” 
bool
””	 #
canExecuteSendMessage
”” #
(
””# $
object
””$ *
	parameter
””+ 4
)
””4 5
{
‘‘ 
return
’’ 
!
’’ 	
string
’’	 
.
’’  
IsNullOrWhiteSpace
’’ "
(
’’" # 
CurrentChatMessage
’’# 5
)
’’5 6
&&
’’7 9&
ChatServiceClientManager
’’: R
.
’’R S
instance
’’S [
.
’’[ \
IsConnected
’’\ g
(
’’g h
)
’’h i
;
’’i j
}
÷÷ 
private
ÿÿ 
void
ÿÿ	  
executeSendMessage
ÿÿ  
(
ÿÿ  !
object
ÿÿ! '
	parameter
ÿÿ( 1
)
ÿÿ1 2
{
ŸŸ 
if
⁄⁄ 
(
⁄⁄ 
!
⁄⁄ #
canExecuteSendMessage
⁄⁄ 
(
⁄⁄ 
	parameter
⁄⁄ %
)
⁄⁄% &
)
⁄⁄& '
return
⁄⁄( .
;
⁄⁄. /
string
‹‹ 
messageToSend
‹‹ 
=
‹‹  
CurrentChatMessage
‹‹ *
;
‹‹* + 
CurrentChatMessage
›› 
=
›› 
string
›› 
.
›› 
Empty
›› "
;
››" #
try
ﬂﬂ 
{
‡‡ 
	chatProxy
·· 

.
··
 #
sendLobbyMessageAsync
··  
(
··  !
SessionService
··! /
.
··/ 0
Username
··0 8
,
··8 9
	LobbyCode
··: C
,
··C D
messageToSend
··E R
)
··R S
;
··S T
Debug
‚‚ 
.
‚‚ 
	WriteLine
‚‚ 
(
‚‚ 
$"
‚‚ 
$str
‚‚ "
{
‚‚" #
messageToSend
‚‚# 0
}
‚‚0 1
$str
‚‚1 2
"
‚‚2 3
)
‚‚3 4
;
‚‚4 5
}
„„ 
catch
‰‰ 
(
‰‰ 
	Exception
‰‰ 
ex
‰‰ 
)
‰‰ 
{
ÂÂ 
Debug
ÊÊ 
.
ÊÊ 
	WriteLine
ÊÊ 
(
ÊÊ 
$"
ÊÊ 
$str
ÊÊ /
{
ÊÊ/ 0
ex
ÊÊ0 2
.
ÊÊ2 3
Message
ÊÊ3 :
}
ÊÊ: ;
"
ÊÊ; <
)
ÊÊ< =
;
ÊÊ= >

MessageBox
ÁÁ 
.
ÁÁ 
Show
ÁÁ 
(
ÁÁ 
$"
ÁÁ 
$str
ÁÁ +
{
ÁÁ+ ,
ex
ÁÁ, .
.
ÁÁ. /
Message
ÁÁ/ 6
}
ÁÁ6 7
"
ÁÁ7 8
,
ÁÁ8 9
Lang
ÁÁ: >
.
ÁÁ> ?

ErrorTitle
ÁÁ? I
,
ÁÁI J
MessageBoxButton
ÁÁK [
.
ÁÁ[ \
OK
ÁÁ\ ^
,
ÁÁ^ _
MessageBoxImage
ÁÁ` o
.
ÁÁo p
Warning
ÁÁp w
)
ÁÁw x
;
ÁÁx y
if
ÈÈ 
(
ÈÈ 
	chatProxy
ÈÈ 
.
ÈÈ 
State
ÈÈ 
==
ÈÈ  
CommunicationState
ÈÈ *
.
ÈÈ* +
Faulted
ÈÈ+ 2
)
ÈÈ2 3
{
ÍÍ  
disconnectFromChat
ÎÎ 
(
ÎÎ 
)
ÎÎ 
;
ÎÎ 
connectToChat
ÏÏ 
(
ÏÏ 
	LobbyCode
ÏÏ 
)
ÏÏ 
;
ÏÏ 
}
ÌÌ 
}
ÓÓ 
}
ÔÔ 
private
ÙÙ 
void
ÙÙ	 
executeLeaveLobby
ÙÙ 
(
ÙÙ  
object
ÙÙ  &
	parameter
ÙÙ' 0
)
ÙÙ0 1
{
ıı 
if
ˆˆ 
(
ˆˆ 
!
ˆˆ -
MatchmakingServiceClientManager
ˆˆ %
.
ˆˆ% &
instance
ˆˆ& .
.
ˆˆ. /
EnsureConnected
ˆˆ/ >
(
ˆˆ> ?
)
ˆˆ? @
)
ˆˆ@ A
return
ˆˆB H
;
ˆˆH I
IsBusy
˜˜ 
=
˜˜ 	
true
˜˜
 
;
˜˜ 
try
¯¯ 
{
˘˘ 
matchmakingProxy
˙˙ 
.
˙˙ 

leaveLobby
˙˙ 
(
˙˙ 
SessionService
˙˙ +
.
˙˙+ ,
Username
˙˙, 4
,
˙˙4 5
	LobbyCode
˙˙6 ?
)
˙˙? @
;
˙˙@ A
navigateBack
˚˚ 
?
˚˚ 
.
˚˚ 
Invoke
˚˚ 
(
˚˚ 
)
˚˚ 
;
˚˚ 
}
¸¸ 
catch
˝˝ 
(
˝˝ 
	Exception
˝˝ 
ex
˝˝ 
)
˝˝ 
{
˝˝ 

MessageBox
˝˝ "
.
˝˝" #
Show
˝˝# '
(
˝˝' (
$"
˝˝( *
$str
˝˝* ?
{
˝˝? @
ex
˝˝@ B
.
˝˝B C
Message
˝˝C J
}
˝˝J K
"
˝˝K L
,
˝˝L M
$str
˝˝N U
)
˝˝U V
;
˝˝V W
}
˝˝X Y
finally
˛˛ 
{
˛˛	 

IsBusy
˛˛ 
=
˛˛ 
false
˛˛ 
;
˛˛ 
}
˛˛ 
}
ˇˇ 
private
ÅÅ 
async
ÅÅ	 
void
ÅÅ 
executeStartGame
ÅÅ $
(
ÅÅ$ %
object
ÅÅ% +
	parameter
ÅÅ, 5
)
ÅÅ5 6
{
ÇÇ 
if
ÉÉ 
(
ÉÉ 
!
ÉÉ 
IsHost
ÉÉ 
||
ÉÉ 
Players
ÉÉ 
.
ÉÉ 
Count
ÉÉ 
<
ÉÉ 
$num
ÉÉ  
)
ÉÉ  !
{
ÑÑ 

MessageBox
ÖÖ 
.
ÖÖ 
Show
ÖÖ 
(
ÖÖ 
$str
ÖÖ 2
,
ÖÖ2 3
$str
ÖÖ4 G
,
ÖÖG H
MessageBoxButton
ÖÖI Y
.
ÖÖY Z
OK
ÖÖZ \
,
ÖÖ\ ]
MessageBoxImage
ÖÖ^ m
.
ÖÖm n
Warning
ÖÖn u
)
ÖÖu v
;
ÖÖv w
return
ÜÜ 
;
ÜÜ 
}
áá 
if
àà 
(
àà 
!
àà -
MatchmakingServiceClientManager
àà %
.
àà% &
instance
àà& .
.
àà. /
EnsureConnected
àà/ >
(
àà> ?
)
àà? @
)
àà@ A
return
ààB H
;
ààH I
IsBusy
ää 
=
ää 	
true
ää
 
;
ää 
try
ãã 
{
åå 
matchmakingProxy
çç 
.
çç 
	startGame
çç 
(
çç 
SessionService
çç *
.
çç* +
Username
çç+ 3
,
çç3 4
	LobbyCode
çç5 >
)
çç> ?
;
çç? @
}
éé 
catch
èè 
(
èè 
	Exception
èè 
ex
èè 
)
èè 
{
èè 

MessageBox
èè "
.
èè" #
Show
èè# '
(
èè' (
$"
èè( *
$str
èè* ?
{
èè? @
ex
èè@ B
.
èèB C
Message
èèC J
}
èèJ K
"
èèK L
,
èèL M
$str
èèN U
)
èèU V
;
èèV W
IsBusy
èèX ^
=
èè_ `
false
èèa f
;
èèf g
}
èèh i
}
êê 
private
íí 
void
íí	 
executeKickPlayer
íí 
(
íí  
object
íí  &
	parameter
íí' 0
)
íí0 1
{
ìì 
if
îî 
(
îî 
!
îî 
(
îî 
	parameter
îî 
is
îî 
string
îî 
playerToKick
îî '
)
îî' (
||
îî) +
!
îî, -
IsHost
îî- 3
||
îî4 6
playerToKick
îî7 C
==
îîD F
HostUsername
îîG S
)
îîS T
return
îîU [
;
îî[ \
if
ïï 
(
ïï 
!
ïï -
MatchmakingServiceClientManager
ïï %
.
ïï% &
instance
ïï& .
.
ïï. /
EnsureConnected
ïï/ >
(
ïï> ?
)
ïï? @
)
ïï@ A
return
ïïB H
;
ïïH I
var
óó 
confirmResult
óó 
=
óó 

MessageBox
óó 
.
óó  
Show
óó  $
(
óó$ %
$"
óó% '
$str
óó' E
{
óóE F
playerToKick
óóF R
}
óóR S
$str
óóS T
"
óóT U
,
óóU V
$str
óóW d
,
óód e
MessageBoxButton
óóf v
.
óóv w
YesNo
óów |
,
óó| }
MessageBoxImageóó~ ç
.óóç é
Warningóóé ï
)óóï ñ
;óóñ ó
if
òò 
(
òò 
confirmResult
òò 
!=
òò 
MessageBoxResult
òò &
.
òò& '
Yes
òò' *
)
òò* +
return
òò, 2
;
òò2 3
try
öö 
{
õõ 
matchmakingProxy
úú 
.
úú 

kickPlayer
úú 
(
úú 
SessionService
úú +
.
úú+ ,
Username
úú, 4
,
úú4 5
playerToKick
úú6 B
,
úúB C
	LobbyCode
úúD M
)
úúM N
;
úúN O
}
ùù 
catch
ûû 
(
ûû 
	Exception
ûû 
ex
ûû 
)
ûû 
{
üü 

MessageBox
†† 
.
†† 
Show
†† 
(
†† 
$"
†† 
$str
†† )
{
††) *
ex
††* ,
.
††, -
Message
††- 4
}
††4 5
"
††5 6
,
††6 7
$str
††8 ?
)
††? @
;
††@ A
}
°° 
}
¢¢ 
private
§§ 
async
§§	 
Task
§§ $
loadOnlineFriendsAsync
§§ *
(
§§* +
)
§§+ ,
{
•• 
if
¶¶ 
(
¶¶ 
!
¶¶ 
IsHost
¶¶ 
||
¶¶ 
!
¶¶ (
SocialServiceClientManager
¶¶ +
.
¶¶+ ,
instance
¶¶, 4
.
¶¶4 5
EnsureConnected
¶¶5 D
(
¶¶D E
SessionService
¶¶E S
.
¶¶S T
Username
¶¶T \
)
¶¶\ ]
)
¶¶] ^
return
¶¶_ e
;
¶¶e f
setBusy
®® 
(
®® 	
true
®®	 
)
®® 
;
®® 
OnlineFriends
©© 
.
©© 
Clear
©© 
(
©© 
)
©© 
;
©© 
try
™™ 
{
´´ "
SocialManagerService
¨¨ 
.
¨¨ 
	FriendDto
¨¨ 
[
¨¨  
]
¨¨  !
friends
¨¨" )
=
¨¨* +
await
¨¨, 1
socialProxy
¨¨2 =
.
¨¨= >!
getFriendsListAsync
¨¨> Q
(
¨¨Q R
SessionService
¨¨R `
.
¨¨` a
Username
¨¨a i
)
¨¨i j
;
¨¨j k
if
≠≠ 
(
≠≠ 
friends
≠≠ 
!=
≠≠ 
null
≠≠ 
)
≠≠ 
{
ÆÆ 
foreach
ØØ 
(
ØØ	 

var
ØØ
 
	friendDto
ØØ 
in
ØØ 
friends
ØØ "
.
ØØ" #
Where
ØØ# (
(
ØØ( )
f
ØØ) *
=>
ØØ+ -
f
ØØ. /
.
ØØ/ 0
isOnline
ØØ0 8
)
ØØ8 9
)
ØØ9 :
{
∞∞ 
OnlineFriends
±± 
.
±± 
Add
±± 
(
±± 
new
±± 
FriendDtoDisplay
±± '
(
±±' (
	friendDto
±±( 1
)
±±1 2
)
±±2 3
;
±±3 4
}
≤≤ 
Console
≥≥ 
.
≥≥ 	
	WriteLine
≥≥	 
(
≥≥ 
$"
≥≥ 
$str
≥≥ 
{
≥≥ 
OnlineFriends
≥≥ *
.
≥≥* +
Count
≥≥+ 0
}
≥≥0 1
$str
≥≥1 W
"
≥≥W X
)
≥≥X Y
;
≥≥Y Z
}
¥¥ 
}
µµ 
catch
∂∂ 
(
∂∂ 
	Exception
∂∂ 
ex
∂∂ 
)
∂∂ 
{
∂∂ 
handleError
∂∂ #
(
∂∂# $
$str
∂∂$ B
,
∂∂B C
ex
∂∂D F
)
∂∂F G
;
∂∂G H
}
∂∂I J
finally
∑∑ 
{
∑∑	 

setBusy
∑∑ 
(
∑∑ 
false
∑∑ 
)
∑∑ 
;
∑∑ 
}
∑∑ 
}
∏∏ 
private
∫∫ 
void
∫∫	 !
executeInviteFriend
∫∫ !
(
∫∫! "
object
∫∫" (
	parameter
∫∫) 2
)
∫∫2 3
{
ªª 
if
ºº 
(
ºº 
!
ºº 
(
ºº 
	parameter
ºº 
is
ºº 
FriendDtoDisplay
ºº $
friendToInvite
ºº% 3
)
ºº3 4
)
ºº4 5
return
ºº6 <
;
ºº< =
if
ΩΩ 
(
ΩΩ 
!
ΩΩ -
MatchmakingServiceClientManager
ΩΩ %
.
ΩΩ% &
instance
ΩΩ& .
.
ΩΩ. /
EnsureConnected
ΩΩ/ >
(
ΩΩ> ?
)
ΩΩ? @
)
ΩΩ@ A
return
ΩΩB H
;
ΩΩH I
try
øø 
{
¿¿ 
Console
¡¡ 
.
¡¡ 	
	WriteLine
¡¡	 
(
¡¡ 
$"
¡¡ 
$str
¡¡ '
{
¡¡' (
friendToInvite
¡¡( 6
.
¡¡6 7
Username
¡¡7 ?
}
¡¡? @
$str
¡¡@ K
{
¡¡K L
	LobbyCode
¡¡L U
}
¡¡U V
"
¡¡V W
)
¡¡W X
;
¡¡X Y
matchmakingProxy
¬¬ 
.
¬¬ 
inviteToLobby
¬¬ 
(
¬¬  
SessionService
¬¬  .
.
¬¬. /
Username
¬¬/ 7
,
¬¬7 8
friendToInvite
¬¬9 G
.
¬¬G H
Username
¬¬H P
,
¬¬P Q
	LobbyCode
¬¬R [
)
¬¬[ \
;
¬¬\ ]

MessageBox
√√ 
.
√√ 
Show
√√ 
(
√√ 
$"
√√ 
$str
√√ &
{
√√& '
friendToInvite
√√' 5
.
√√5 6
Username
√√6 >
}
√√> ?
$str
√√? @
"
√√@ A
,
√√A B
$str
√√C T
,
√√T U
MessageBoxButton
√√V f
.
√√f g
OK
√√g i
,
√√i j
MessageBoxImage
√√k z
.
√√z {
Information√√{ Ü
)√√Ü á
;√√á à
}
ƒƒ 
catch
≈≈ 
(
≈≈ 
	Exception
≈≈ 
ex
≈≈ 
)
≈≈ 
{
∆∆ 
handleError
«« 
(
«« 
$str
«« )
,
««) *
ex
««+ -
)
««- .
;
««. /
}
»» 
}
…… 
private
   
void
  	 "
subscribeToCallbacks
   "
(
  " #
)
  # $
{
ÀÀ 
if
ÃÃ 
(
ÃÃ (
matchmakingCallbackHandler
ÃÃ 
!=
ÃÃ  "
null
ÃÃ# '
)
ÃÃ' (
{
ÕÕ (
matchmakingCallbackHandler
ŒŒ 
.
ŒŒ 
LobbyStateUpdated
ŒŒ -
-=
ŒŒ. 0$
handleLobbyStateUpdate
ŒŒ1 G
;
ŒŒG H(
matchmakingCallbackHandler
œœ 
.
œœ 

MatchFound
œœ &
-=
œœ' )
handleMatchFound
œœ* :
;
œœ: ;(
matchmakingCallbackHandler
–– 
.
–– 
KickedFromLobby
–– +
-=
––, .#
handleKickedFromLobby
––/ D
;
––D E(
matchmakingCallbackHandler
““ 
.
““ 
LobbyStateUpdated
““ -
+=
““. 0$
handleLobbyStateUpdate
““1 G
;
““G H(
matchmakingCallbackHandler
”” 
.
”” 

MatchFound
”” &
+=
””' )
handleMatchFound
””* :
;
””: ;(
matchmakingCallbackHandler
‘‘ 
.
‘‘ 
KickedFromLobby
‘‘ +
+=
‘‘, .#
handleKickedFromLobby
‘‘/ D
;
‘‘D E
Debug
’’ 
.
’’ 
	WriteLine
’’ 
(
’’ 
$str
’’ 7
)
’’7 8
;
’’8 9
}
÷÷ 
else
◊◊ 
{
◊◊ 
Debug
◊◊ 
.
◊◊ 
	WriteLine
◊◊ 
(
◊◊ 
$str
◊◊ F
)
◊◊F G
;
◊◊G H
}
◊◊I J
}
ÿÿ 
private
⁄⁄ 
void
⁄⁄	 &
unsubscribeFromCallbacks
⁄⁄ &
(
⁄⁄& '
)
⁄⁄' (
{
€€ 
if
‹‹ 
(
‹‹ (
matchmakingCallbackHandler
‹‹ 
!=
‹‹  "
null
‹‹# '
)
‹‹' (
{
›› (
matchmakingCallbackHandler
ﬁﬁ 
.
ﬁﬁ 
LobbyStateUpdated
ﬁﬁ -
-=
ﬁﬁ. 0$
handleLobbyStateUpdate
ﬁﬁ1 G
;
ﬁﬁG H(
matchmakingCallbackHandler
ﬂﬂ 
.
ﬂﬂ 

MatchFound
ﬂﬂ &
-=
ﬂﬂ' )
handleMatchFound
ﬂﬂ* :
;
ﬂﬂ: ;(
matchmakingCallbackHandler
‡‡ 
.
‡‡ 
KickedFromLobby
‡‡ +
-=
‡‡, .#
handleKickedFromLobby
‡‡/ D
;
‡‡D E
Debug
·· 
.
·· 
	WriteLine
·· 
(
·· 
$str
·· ;
)
··; <
;
··< =
}
‚‚ *
unsubscribeFromChatCallbacks
„„ 
(
„„ 
)
„„ 
;
„„  
}
‰‰ 
private
ÊÊ 
void
ÊÊ	 $
handleLobbyStateUpdate
ÊÊ $
(
ÊÊ$ %
LobbyStateDto
ÊÊ% 2
newState
ÊÊ3 ;
)
ÊÊ; <
{
ÁÁ 
if
ËË 
(
ËË 
newState
ËË 
!=
ËË 
null
ËË 
&&
ËË 
(
ËË 
string
ËË  
.
ËË  !
IsNullOrEmpty
ËË! .
(
ËË. /
this
ËË/ 3
.
ËË3 4
	LobbyCode
ËË4 =
)
ËË= >
||
ËË? A
this
ËËB F
.
ËËF G
	LobbyCode
ËËG P
==
ËËQ S
$str
ËËT `
||
ËËa c
newState
ËËd l
.
ËËl m
lobbyId
ËËm t
==
ËËu w
this
ËËx |
.
ËË| }
	LobbyCodeËË} Ü
)ËËÜ á
)ËËá à
{
ÈÈ 
bool
ÍÍ 
isInitialJoin
ÍÍ 
=
ÍÍ 
string
ÍÍ 
.
ÍÍ 
IsNullOrEmpty
ÍÍ *
(
ÍÍ* +
this
ÍÍ+ /
.
ÍÍ/ 0
	LobbyCode
ÍÍ0 9
)
ÍÍ9 :
||
ÍÍ; =
this
ÍÍ> B
.
ÍÍB C
	LobbyCode
ÍÍC L
==
ÍÍM O
$str
ÍÍP \
;
ÍÍ\ ]
Application
ÎÎ 
.
ÎÎ 
Current
ÎÎ 
.
ÎÎ 

Dispatcher
ÎÎ 
.
ÎÎ  
Invoke
ÎÎ  &
(
ÎÎ& '
(
ÎÎ' (
)
ÎÎ( )
=>
ÎÎ* ,
updateState
ÎÎ- 8
(
ÎÎ8 9
newState
ÎÎ9 A
)
ÎÎA B
)
ÎÎB C
;
ÎÎC D
if
ÌÌ 
(
ÌÌ 
isInitialJoin
ÌÌ 
&&
ÌÌ 
!
ÌÌ 
string
ÌÌ 
.
ÌÌ 
IsNullOrEmpty
ÌÌ +
(
ÌÌ+ ,
newState
ÌÌ, 4
.
ÌÌ4 5
lobbyId
ÌÌ5 <
)
ÌÌ< =
)
ÌÌ= >
{
ÓÓ 
Debug
ÔÔ 
.
ÔÔ 
	WriteLine
ÔÔ 
(
ÔÔ 
$"
ÔÔ 
$str
ÔÔ 1
{
ÔÔ1 2
newState
ÔÔ2 :
.
ÔÔ: ;
lobbyId
ÔÔ; B
}
ÔÔB C
$str
ÔÔC Y
"
ÔÔY Z
)
ÔÔZ [
;
ÔÔ[ \
connectToChat
 
(
 
newState
 
.
 
lobbyId
 
)
  
;
  !
}
ÒÒ 
}
ÚÚ 
}
ÛÛ 
private
ıı 
void
ıı	 
handleMatchFound
ıı 
(
ıı 
string
ıı %
matchId
ıı& -
,
ıı- .
List
ıı/ 3
<
ıı3 4
string
ıı4 :
>
ıı: ;
playerUsernames
ıı< K
)
ııK L
{
ˆˆ 
if
˜˜ 
(
˜˜ 
playerUsernames
˜˜ 
!=
˜˜ 
null
˜˜ 
&&
˜˜ 
playerUsernames
˜˜  /
.
˜˜/ 0
Contains
˜˜0 8
(
˜˜8 9
SessionService
˜˜9 G
.
˜˜G H
Username
˜˜H P
)
˜˜P Q
)
˜˜Q R
{
¯¯ 
Application
˘˘ 
.
˘˘ 
Current
˘˘ 
.
˘˘ 

Dispatcher
˘˘ 
.
˘˘  
Invoke
˘˘  &
(
˘˘& '
(
˘˘' (
)
˘˘( )
=>
˘˘* ,
{
˙˙ 
IsBusy
˚˚ 
=
˚˚ 	
false
˚˚
 
;
˚˚ 

MessageBox
¸¸ 
.
¸¸ 
Show
¸¸ 
(
¸¸ 
$"
¸¸ 
$str
¸¸ 3
{
¸¸3 4
matchId
¸¸4 ;
}
¸¸; <
$str
¸¸< @
"
¸¸@ A
,
¸¸A B
$str
¸¸C P
,
¸¸P Q
MessageBoxButton
¸¸R b
.
¸¸b c
OK
¸¸c e
,
¸¸e f
MessageBoxImage
¸¸g v
.
¸¸v w
Information¸¸w Ç
)¸¸Ç É
;¸¸É Ñ&
unsubscribeFromCallbacks
˝˝ 
(
˝˝ 
)
˝˝ 
;
˝˝ 
var
ˇˇ 

gameWindow
ˇˇ 
=
ˇˇ 
new
ˇˇ 

GameWindow
ˇˇ  
(
ˇˇ  !
)
ˇˇ! "
;
ˇˇ" #

gameWindow
ÄÄ 
.
ÄÄ 
Show
ÄÄ 
(
ÄÄ 
)
ÄÄ 
;
ÄÄ 
var
ÇÇ 
currentWindow
ÇÇ 
=
ÇÇ 
Application
ÇÇ  
.
ÇÇ  !
Current
ÇÇ! (
.
ÇÇ( )
Windows
ÇÇ) 0
.
ÇÇ0 1
OfType
ÇÇ1 7
<
ÇÇ7 8
Window
ÇÇ8 >
>
ÇÇ> ?
(
ÇÇ? @
)
ÇÇ@ A
.
ÇÇA B
FirstOrDefault
ÇÇB P
(
ÇÇP Q
w
ÇÇQ R
=>
ÇÇS U
w
ÇÇV W
.
ÇÇW X
IsActive
ÇÇX `
)
ÇÇ` a
;
ÇÇa b
currentWindow
ÉÉ 
?
ÉÉ 
.
ÉÉ 
Close
ÉÉ 
(
ÉÉ 
)
ÉÉ 
;
ÉÉ 
}
ÑÑ 
)
ÑÑ 
;
ÑÑ 
}
ÖÖ 
}
ÜÜ 
private
àà 
void
àà	 #
handleKickedFromLobby
àà #
(
àà# $
string
àà$ *
reason
àà+ 1
)
àà1 2
{
ââ 
Application
ää 
.
ää 
Current
ää 
.
ää 

Dispatcher
ää 
.
ää  
Invoke
ää  &
(
ää& '
(
ää' (
)
ää( )
=>
ää* ,
{
ãã 

MessageBox
åå 
.
åå 
Show
åå 
(
åå 
$"
åå 
$str
åå 1
{
åå1 2
reason
åå2 8
}
åå8 9
"
åå9 :
,
åå: ;
$str
åå< D
,
ååD E
MessageBoxButton
ååF V
.
ååV W
OK
ååW Y
,
ååY Z
MessageBoxImage
åå[ j
.
ååj k
Warning
ååk r
)
åår s
;
åås t&
unsubscribeFromCallbacks
çç 
(
çç 
)
çç 
;
çç 
navigateBack
éé 
?
éé 
.
éé 
Invoke
éé 
(
éé 
)
éé 
;
éé 
}
èè 
)
èè 
;
èè 
}
êê 
private
íí 
void
íí	 
updateState
íí 
(
íí 
LobbyStateDto
íí '
state
íí( -
)
íí- .
{
ìì 
	LobbyCode
ïï 

=
ïï 
state
ïï 
.
ïï 
lobbyId
ïï 
;
ïï 
HostUsername
ññ 
=
ññ 
state
ññ 
.
ññ 
hostUsername
ññ "
;
ññ" #
CurrentSettings
óó 
=
óó 
state
óó 
.
óó  
currentSettingsDto
óó +
;
óó+ ,
var
ôô 
playersToRemove
ôô 
=
ôô 
Players
ôô 
.
ôô 
Except
ôô %
(
ôô% &
state
ôô& +
.
ôô+ ,
players
ôô, 3
)
ôô3 4
.
ôô4 5
ToList
ôô5 ;
(
ôô; <
)
ôô< =
;
ôô= >
var
öö 
playersToAdd
öö 
=
öö 
state
öö 
.
öö 
players
öö !
.
öö! "
Except
öö" (
(
öö( )
Players
öö) 0
)
öö0 1
.
öö1 2
ToList
öö2 8
(
öö8 9
)
öö9 :
;
öö: ;
foreach
úú 
(
úú	 

var
úú
 
p
úú 
in
úú 
playersToRemove
úú "
)
úú" #
Players
úú$ +
.
úú+ ,
Remove
úú, 2
(
úú2 3
p
úú3 4
)
úú4 5
;
úú5 6
foreach
ùù 
(
ùù	 

var
ùù
 
p
ùù 
in
ùù 
playersToAdd
ùù 
)
ùù  
Players
ùù! (
.
ùù( )
Add
ùù) ,
(
ùù, -
p
ùù- .
)
ùù. /
;
ùù/ 0
OnPropertyChanged
üü 
(
üü 
nameof
üü 
(
üü 
IsHost
üü  
)
üü  !
)
üü! "
;
üü" #
OnPropertyChanged
†† 
(
†† 
nameof
†† 
(
†† 
IsGuestUser
†† %
)
††% &
)
††& '
;
††' (.
 RaiseCanExecuteChangedOnCommands
°° !
(
°°! "
)
°°" #
;
°°# $
}
¢¢ 
private
§§ 
void
§§	 .
 RaiseCanExecuteChangedOnCommands
§§ .
(
§§. /
)
§§/ 0
{
•• 
Application
¶¶ 
.
¶¶ 
Current
¶¶ 
.
¶¶ 

Dispatcher
¶¶ 
?
¶¶  
.
¶¶  !
Invoke
¶¶! '
(
¶¶' (
(
¶¶( )
)
¶¶) *
=>
¶¶+ -
{
ßß 
(
®® 
(
®® 
RelayCommand
®® 
)
®® 
LeaveLobbyCommand
®® !
)
®®! "
.
®®" #$
RaiseCanExecuteChanged
®®# 9
(
®®9 :
)
®®: ;
;
®®; <
(
©© 
(
©© 
RelayCommand
©© 
)
©© 
StartGameCommand
©©  
)
©©  !
.
©©! "$
RaiseCanExecuteChanged
©©" 8
(
©©8 9
)
©©9 :
;
©©: ;
(
™™ 
(
™™ 
RelayCommand
™™ 
)
™™ !
InviteFriendCommand
™™ #
)
™™# $
.
™™$ %$
RaiseCanExecuteChanged
™™% ;
(
™™; <
)
™™< =
;
™™= >
(
´´ 
(
´´ 
RelayCommand
´´ 
)
´´ 
KickPlayerCommand
´´ !
)
´´! "
.
´´" #$
RaiseCanExecuteChanged
´´# 9
(
´´9 :
)
´´: ;
;
´´; <
(
¨¨ 
(
¨¨ 
RelayCommand
¨¨ 
)
¨¨  
UploadImageCommand
¨¨ "
)
¨¨" #
.
¨¨# $$
RaiseCanExecuteChanged
¨¨$ :
(
¨¨: ;
)
¨¨; <
;
¨¨< =
(
≠≠ 
(
≠≠ 
RelayCommand
≠≠ 
)
≠≠ #
ChangeSettingsCommand
≠≠ %
)
≠≠% &
.
≠≠& '$
RaiseCanExecuteChanged
≠≠' =
(
≠≠= >
)
≠≠> ?
;
≠≠? @
(
ÆÆ 
(
ÆÆ 
RelayCommand
ÆÆ 
)
ÆÆ #
RefreshFriendsCommand
ÆÆ %
)
ÆÆ% &
.
ÆÆ& '$
RaiseCanExecuteChanged
ÆÆ' =
(
ÆÆ= >
)
ÆÆ> ?
;
ÆÆ? @
(
ØØ 
(
ØØ 
RelayCommand
ØØ 
)
ØØ  
InviteGuestCommand
ØØ "
)
ØØ" #
.
ØØ# $$
RaiseCanExecuteChanged
ØØ$ :
(
ØØ: ;
)
ØØ; <
;
ØØ< =
}
∞∞ 
)
∞∞ 
;
∞∞ 
}
±± 
private
≤≤ 
void
≤≤	 
setBusy
≤≤ 
(
≤≤ 
bool
≤≤ 
busy
≤≤ 
)
≤≤  
{
≥≥ 
IsBusy
¥¥ 
=
¥¥ 	
busy
¥¥
 
;
¥¥ 
}
µµ 
private
∑∑ 
void
∑∑	 
handleError
∑∑ 
(
∑∑ 
string
∑∑  
message
∑∑! (
,
∑∑( )
	Exception
∑∑* 3
ex
∑∑4 6
)
∑∑6 7
{
∏∏ 
Console
ππ 
.
ππ 	
	WriteLine
ππ	 
(
ππ 
$"
ππ 
$str
ππ 
{
ππ 
message
ππ !
}
ππ! "
$str
ππ" $
{
ππ$ %
ex
ππ% '
}
ππ' (
"
ππ( )
)
ππ) *
;
ππ* +

MessageBox
∫∫ 
.
∫∫ 
Show
∫∫ 
(
∫∫ 
$"
∫∫ 
{
∫∫ 
message
∫∫ 
}
∫∫ 
$str
∫∫ 
{
∫∫ 
ex
∫∫ !
.
∫∫! "
Message
∫∫" )
}
∫∫) *
"
∫∫* +
,
∫∫+ ,
$str
∫∫- 4
,
∫∫4 5
MessageBoxButton
∫∫6 F
.
∫∫F G
OK
∫∫G I
,
∫∫I J
MessageBoxImage
∫∫K Z
.
∫∫Z [
Error
∫∫[ `
)
∫∫` a
;
∫∫a b
}
ªª 
public
ΩΩ 
void
ΩΩ 
cleanup
ΩΩ 
(
ΩΩ 
)
ΩΩ 
{
ææ 
Debug
øø 
.
øø 
	WriteLine
øø 
(
øø 
$str
øø 1
)
øø1 2
;
øø2 3&
unsubscribeFromCallbacks
¿¿ 
(
¿¿ 
)
¿¿ 
;
¿¿  
disconnectFromChat
¡¡ 
(
¡¡ 
)
¡¡ 
;
¡¡ 
}
¬¬ 
}
√√ 
}ƒƒ ≤
ùC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\ViewModel\Game\ChatMessageDisplayViewModel.cs
	namespace 	
MindWeaveClient
 
. 
	ViewModel #
.# $
Game$ (
{ 
public		 

class		 '
ChatMessageDisplayViewModel		 ,
:		- .
BaseViewModel		/ <
{

 
public 
string 
SenderUsername $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 
string 
Content 
{ 
get  #
;# $
set% (
;( )
}* +
public 
DateTime 
	Timestamp !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
HorizontalAlignment "
	Alignment# ,
{- .
get/ 2
;2 3
set4 7
;7 8
}9 :
public 
ICommand  
ReportMessageCommand ,
{- .
get/ 2
;2 3
}4 5
public '
ChatMessageDisplayViewModel *
(* +
ChatMessageDto+ 9
dto: =
)= >
{ 	
SenderUsername 
= 
dto  
.  !
senderUsername! /
;/ 0
Content 
= 
dto 
. 
content !
;! "
	Timestamp 
= 
dto 
. 
	timestamp %
.% &
ToLocalTime& 1
(1 2
)2 3
;3 4
	Alignment 
= 
dto 
. 
senderUsername *
.* +
Equals+ 1
(1 2
SessionService2 @
.@ A
UsernameA I
,I J
StringComparisonK [
.[ \
OrdinalIgnoreCase\ m
)m n
? 
HorizontalAlignment 1
.1 2
Right2 7
: 
HorizontalAlignment 1
.1 2
Left2 6
;6 7 
ReportMessageCommand  
=! "
new# &
RelayCommand' 3
(3 4
param4 9
=>: < 
executeReportMessage= Q
(Q R
)R S
,S T
paramU Z
=>[ ]#
canExecuteReportMessage^ u
(u v
)v w
)w x
;x y
} 	
private 
bool #
canExecuteReportMessage ,
(, -
)- .
=>/ 1
SenderUsername2 @
!=A C
SessionServiceD R
.R S
UsernameS [
;[ \
private 
void  
executeReportMessage )
() *
)* +
{ 	

MessageBox   
.   
Show   
(   
$"   
$str   E
{  E F
SenderUsername  F T
}  T U
$str  U Y
{  Y Z
Content  Z a
}  a b
$str	  b à
"
  à â
,
  â ä
$str
  ã õ
)
  õ ú
;
  ú ù
}!! 	
}"" 
}## ≠
äC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\ViewModel\BaseViewModel.cs
	namespace 	
MindWeaveClient
 
. 
	ViewModel #
{ 
public 

class 
BaseViewModel 
:  "
INotifyPropertyChanged! 7
{		 
public

 
event

 '
PropertyChangedEventHandler

 0
PropertyChanged

1 @
;

@ A
	protected 
void 
OnPropertyChanged (
(( )
[) *
CallerMemberName* :
]: ;
string< B
propertyNameC O
=P Q
nullR V
)V W
{ 	
PropertyChanged 
? 
. 
Invoke #
(# $
this$ (
,( )
new* -$
PropertyChangedEventArgs. F
(F G
propertyNameG S
)S T
)T U
;U V
} 	
	protected 
void 
OnCanExecuteChanged *
(* +
ICommand+ 3
command4 ;
); <
{ 	
Application 
. 
Current 
.  

Dispatcher  *
.* +
Invoke+ 1
(1 2
(2 3
)3 4
=>5 7
CommandManager8 F
.F G&
InvalidateRequerySuggestedG a
(a b
)b c
)c d
;d e
} 	
} 
} ü™
•C:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\ViewModel\Authentication\PasswordRecoveryViewModel.cs
	namespace

 	
MindWeaveClient


 
.

 
	ViewModel

 #
.

# $
Authentication

$ 2
{ 
public 

class %
PasswordRecoveryViewModel *
:+ ,
BaseViewModel- :
{ 
private 
readonly 
Action 
navigateBack  ,
;, -
private 
readonly 
Action 
navigateToLogin  /
;/ 0
private 
readonly '
AuthenticationManagerClient 4

authClient5 ?
;? @
private 
bool 
isStep1VisibleValue (
=) *
true+ /
;/ 0
private 
bool 
isStep2VisibleValue (
;( )
private 
bool 
isStep3VisibleValue (
;( )
private 
bool 
isBusyValue  
;  !
public 
bool 
IsStep1Visible "
{# $
get% (
=>) +
isStep1VisibleValue, ?
;? @
setA D
{E F
isStep1VisibleValueG Z
=[ \
value] b
;b c
OnPropertyChangedd u
(u v
)v w
;w x
}y z
}{ |
public 
bool 
IsStep2Visible "
{# $
get% (
=>) +
isStep2VisibleValue, ?
;? @
setA D
{E F
isStep2VisibleValueG Z
=[ \
value] b
;b c
OnPropertyChangedd u
(u v
)v w
;w x
}y z
}{ |
public 
bool 
IsStep3Visible "
{# $
get% (
=>) +
isStep3VisibleValue, ?
;? @
setA D
{E F
isStep3VisibleValueG Z
=[ \
value] b
;b c
OnPropertyChangedd u
(u v
)v w
;w x
}y z
}{ |
public 
bool 
IsBusy 
{ 
get  
=>! #
isBusyValue$ /
;/ 0
private1 8
set9 <
{= >
isBusyValue? J
=K L
valueM R
;R S
OnPropertyChangedT e
(e f
)f g
;g h"
raiseCanExecuteChangedi 
(	 Ä
)
Ä Å
;
Å Ç
}
É Ñ
}
Ö Ü
private 
string 

emailValue !
;! "
public 
string 
Email 
{ 	
get 
=> 

emailValue 
; 
set   
{   

emailValue   
=   
value   $
;  $ %
OnPropertyChanged  & 7
(  7 8
)  8 9
;  9 :"
raiseCanExecuteChanged  ; Q
(  Q R
)  R S
;  S T
}  U V
}!! 	
private## 
string## !
verificationCodeValue## ,
;##, -
public$$ 
string$$ 
VerificationCode$$ &
{%% 	
get&& 
=>&& !
verificationCodeValue&& (
;&&( )
set'' 
{(( 
if)) 
()) 
value)) 
!=)) 
null)) !
&&))" $
!))% &
Regex))& +
.))+ ,
IsMatch)), 3
())3 4
value))4 9
,))9 :
$str)); E
)))E F
)))F G
{** 
value++ 
=++ 
Regex++ !
.++! "
Match++" '
(++' (!
verificationCodeValue++( =
??++> @
$str++A C
,++C D
$str++E N
)++N O
.++O P
Value++P U
;++U V
},, !
verificationCodeValue-- %
=--& '
value--( -
;--- .
OnPropertyChanged.. !
(..! "
).." #
;..# $"
raiseCanExecuteChanged// &
(//& '
)//' (
;//( )
}00 
}11 	
private33 
string33 
newPasswordValue33 '
;33' (
private44 
string44  
confirmPasswordValue44 +
;44+ ,
public55 
string55 
NewPassword55 !
{55" #
get55$ '
=>55( *
newPasswordValue55+ ;
;55; <
set55= @
{55A B
newPasswordValue55C S
=55T U
value55V [
;55[ \
OnPropertyChanged55] n
(55n o
)55o p
;55p q#
raiseCanExecuteChanged	55r à
(
55à â
)
55â ä
;
55ä ã
}
55å ç
}
55é è
public66 
string66 
ConfirmPassword66 %
{66& '
get66( +
=>66, . 
confirmPasswordValue66/ C
;66C D
set66E H
{66I J 
confirmPasswordValue66K _
=66` a
value66b g
;66g h
OnPropertyChanged66i z
(66z {
)66{ |
;66| }#
raiseCanExecuteChanged	66~ î
(
66î ï
)
66ï ñ
;
66ñ ó
}
66ò ô
}
66ö õ
public88 
ICommand88 
SendCodeCommand88 '
{88( )
get88* -
;88- .
}88/ 0
public99 
ICommand99 
VerifyCodeCommand99 )
{99* +
get99, /
;99/ 0
}991 2
public:: 
ICommand:: 
ResendCodeCommand:: )
{::* +
get::, /
;::/ 0
}::1 2
public;; 
ICommand;; 
SavePasswordCommand;; +
{;;, -
get;;. 1
;;;1 2
};;3 4
public<< 
ICommand<< 
GoBackCommand<< %
{<<& '
get<<( +
;<<+ ,
}<<- .
public>> 
bool>> 
CanSendCode>> 
=>>>  "
!>># $
IsBusy>>$ *
&&>>+ -
!>>. /
string>>/ 5
.>>5 6
IsNullOrWhiteSpace>>6 H
(>>H I
Email>>I N
)>>N O
;>>O P
public?? 
bool?? 
CanVerifyCode?? !
=>??" $
!??% &
IsBusy??& ,
&&??- /
VerificationCode??0 @
???@ A
.??A B
Length??B H
==??I K
$num??L M
&&??N P
VerificationCode??Q a
.??a b
All??b e
(??e f
char??f j
.??j k
IsDigit??k r
)??r s
;??s t
public@@ 
bool@@ 
CanResendCode@@ !
=>@@" $
!@@% &
IsBusy@@& ,
&&@@- /
!@@0 1
string@@1 7
.@@7 8
IsNullOrWhiteSpace@@8 J
(@@J K
Email@@K P
)@@P Q
;@@Q R
publicAA 
boolAA 
CanSavePasswordAA #
=>AA$ &
!AA' (
IsBusyAA( .
&&AA/ 1
!BB' (
stringBB( .
.BB. /
IsNullOrEmptyBB/ <
(BB< =
NewPasswordBB= H
)BBH I
&&BBJ L
!CC' (
stringCC( .
.CC. /
IsNullOrEmptyCC/ <
(CC< =
ConfirmPasswordCC= L
)CCL M
&&CCN P
NewPasswordDD' 2
==DD3 5
ConfirmPasswordDD6 E
;DDE F
publicFF %
PasswordRecoveryViewModelFF (
(FF( )
ActionFF) /
navigateBackActionFF0 B
,FFB C
ActionFFD J!
navigateToLoginActionFFK `
)FF` a
{GG 	
navigateBackHH 
=HH 
navigateBackActionHH -
;HH- .
navigateToLoginII 
=II !
navigateToLoginActionII 3
;II3 4

authClientJJ 
=JJ 
newJJ '
AuthenticationManagerClientJJ 8
(JJ8 9
)JJ9 :
;JJ: ;
SendCodeCommandLL 
=LL 
newLL !
RelayCommandLL" .
(LL. /
asyncLL/ 4
paramLL5 :
=>LL; =
awaitLL> C 
executeSendCodeAsyncLLD X
(LLX Y
)LLY Z
,LLZ [
paramLL\ a
=>LLb d
CanSendCodeLLe p
)LLp q
;LLq r
VerifyCodeCommandMM 
=MM 
newMM  #
RelayCommandMM$ 0
(MM0 1
asyncMM1 6
paramMM7 <
=>MM= ?
awaitMM@ E"
executeVerifyCodeAsyncMMF \
(MM\ ]
)MM] ^
,MM^ _
paramMM` e
=>MMf h
CanVerifyCodeMMi v
)MMv w
;MMw x
ResendCodeCommandNN 
=NN 
newNN  #
RelayCommandNN$ 0
(NN0 1
asyncNN1 6
paramNN7 <
=>NN= ?
awaitNN@ E 
executeSendCodeAsyncNNF Z
(NNZ [
trueNN[ _
)NN_ `
,NN` a
paramNNb g
=>NNh j
CanResendCodeNNk x
)NNx y
;NNy z
SavePasswordCommandOO 
=OO  !
newOO" %
RelayCommandOO& 2
(OO2 3
asyncOO3 8
paramOO9 >
=>OO? A
awaitOOB G$
executeSavePasswordAsyncOOH `
(OO` a
)OOa b
,OOb c
paramOOd i
=>OOj l
CanSavePasswordOOm |
)OO| }
;OO} ~
GoBackCommandPP 
=PP 
newPP 
RelayCommandPP  ,
(PP, -
paramPP- 2
=>PP3 5
executeGoBackPP6 C
(PPC D
)PPD E
)PPE F
;PPF G
}QQ 	
privateSS 
asyncSS 
TaskSS  
executeSendCodeAsyncSS /
(SS/ 0
boolSS0 4
isResendSS5 =
=SS> ?
falseSS@ E
)SSE F
{TT 	
ifUU 
(UU 
!UU 
CanSendCodeUU 
&&UU 
!UU  !
isResendUU! )
)UU) *
returnUU+ 1
;UU1 2
ifVV 
(VV 
isResendVV 
&&VV 
!VV 
CanResendCodeVV *
)VV* +
returnVV, 2
;VV2 3
setBusyXX 
(XX 
trueXX 
)XX 
;XX 
tryYY 
{ZZ 
OperationResultDto[[ "
result[[# )
=[[* +
await[[, 1

authClient[[2 <
.[[< =)
sendPasswordRecoveryCodeAsync[[= Z
([[Z [
Email[[[ `
)[[` a
;[[a b
if\\ 
(\\ 
result\\ 
.\\ 
success\\ "
)\\" #
{]] 

MessageBox^^ 
.^^ 
Show^^ #
(^^# $
result^^$ *
.^^* +
message^^+ 2
,^^2 3
Lang^^4 8
.^^8 9
InfoMsgTitleSuccess^^9 L
,^^L M
MessageBoxButton^^N ^
.^^^ _
OK^^_ a
,^^a b
MessageBoxImage^^c r
.^^r s
Information^^s ~
)^^~ 
;	^^ Ä
IsStep1Visible__ "
=__# $
false__% *
;__* +
IsStep2Visible`` "
=``# $
true``% )
;``) *
IsStep3Visibleaa "
=aa# $
falseaa% *
;aa* +
VerificationCodebb $
=bb% &
stringbb' -
.bb- .
Emptybb. 3
;bb3 4
}cc 
elsedd 
{ee 

MessageBoxff 
.ff 
Showff #
(ff# $
resultff$ *
.ff* +
messageff+ 2
,ff2 3
Langff4 8
.ff8 9

ErrorTitleff9 C
,ffC D
MessageBoxButtonffE U
.ffU V
OKffV X
,ffX Y
MessageBoxImageffZ i
.ffi j
Errorffj o
)ffo p
;ffp q
}gg 
}hh 
catchii 
(ii 
	Exceptionii 
exii 
)ii  
{jj 

MessageBoxkk 
.kk 
Showkk 
(kk  
$"kk  "
$strkk" )
{kk) *
exkk* ,
.kk, -
Messagekk- 4
}kk4 5
"kk5 6
,kk6 7
Langkk8 <
.kk< =

ErrorTitlekk= G
,kkG H
MessageBoxButtonkkI Y
.kkY Z
OKkkZ \
,kk\ ]
MessageBoxImagekk^ m
.kkm n
Errorkkn s
)kks t
;kkt u
}ll 
finallymm 
{nn 
setBusyoo 
(oo 
falseoo 
)oo 
;oo 
}pp 
}qq 	
privaterr 
asyncrr 
Taskrr "
executeVerifyCodeAsyncrr 1
(rr1 2
)rr2 3
{ss 	
iftt 
(tt 
!tt 
CanVerifyCodett 
)tt 
returntt  &
;tt& '
setBusyvv 
(vv 
truevv 
)vv 
;vv 
tryww 
{xx 
awaityy 
Taskyy 
.yy 
Delayyy  
(yy  !
$numyy! $
)yy$ %
;yy% &
IsStep1Visible{{ 
={{  
false{{! &
;{{& '
IsStep2Visible|| 
=||  
false||! &
;||& '
IsStep3Visible}} 
=}}  
true}}! %
;}}% &
NewPassword~~ 
=~~ 
$str~~  
;~~  !
ConfirmPassword 
=  !
$str" $
;$ %
}
ÄÄ 
finally
ÅÅ 
{
ÇÇ 
setBusy
ÉÉ 
(
ÉÉ 
false
ÉÉ 
)
ÉÉ 
;
ÉÉ 
}
ÑÑ 
}
ÖÖ 	
private
áá 
async
áá 
Task
áá &
executeSavePasswordAsync
áá 3
(
áá3 4
)
áá4 5
{
àà 	
if
ââ 
(
ââ 
!
ââ 
CanSavePassword
ââ  
)
ââ  !
{
ää 
if
ãã 
(
ãã 
string
ãã 
.
ãã 
IsNullOrEmpty
ãã (
(
ãã( )
NewPassword
ãã) 4
)
ãã4 5
||
ãã6 8
string
ãã9 ?
.
ãã? @
IsNullOrEmpty
ãã@ M
(
ããM N
ConfirmPassword
ããN ]
)
ãã] ^
)
ãã^ _
{
åå 

MessageBox
çç 
.
çç 
Show
çç #
(
çç# $
Lang
çç$ (
.
çç( )*
GlobalErrorAllFieldsRequired
çç) E
,
ççE F
Lang
ççG K
.
ççK L

ErrorTitle
ççL V
,
ççV W
MessageBoxButton
ççX h
.
ççh i
OK
ççi k
,
ççk l
MessageBoxImage
ççm |
.
çç| }
Warningçç} Ñ
)ççÑ Ö
;ççÖ Ü
}
éé 
else
èè 
if
èè 
(
èè 
NewPassword
èè $
!=
èè% '
ConfirmPassword
èè( 7
)
èè7 8
{
êê 

MessageBox
ëë 
.
ëë 
Show
ëë #
(
ëë# $
Lang
ëë$ (
.
ëë( )+
ValidationPasswordsDoNotMatch
ëë) F
,
ëëF G
Lang
ëëH L
.
ëëL M

ErrorTitle
ëëM W
,
ëëW X
MessageBoxButton
ëëY i
.
ëëi j
OK
ëëj l
,
ëël m
MessageBoxImage
ëën }
.
ëë} ~
Warningëë~ Ö
)ëëÖ Ü
;ëëÜ á
}
íí 
return
ìì 
;
ìì 
}
îî 
setBusy
óó 
(
óó 
true
óó 
)
óó 
;
óó 
try
òò 
{
ôô  
OperationResultDto
öö "
result
öö# )
=
öö* +
await
öö, 1

authClient
öö2 <
.
öö< =(
resetPasswordWithCodeAsync
öö= W
(
ööW X
Email
ööX ]
,
öö] ^
VerificationCode
öö_ o
,
ööo p
NewPassword
ööq |
)
öö| }
;
öö} ~
if
úú 
(
úú 
result
úú 
.
úú 
success
úú "
)
úú" #
{
ùù 

MessageBox
ûû 
.
ûû 
Show
ûû #
(
ûû# $
result
ûû$ *
.
ûû* +
message
ûû+ 2
,
ûû2 3
Lang
ûû4 8
.
ûû8 9!
InfoMsgTitleSuccess
ûû9 L
,
ûûL M
MessageBoxButton
ûûN ^
.
ûû^ _
OK
ûû_ a
,
ûûa b
MessageBoxImage
ûûc r
.
ûûr s
Information
ûûs ~
)
ûû~ 
;ûû Ä
navigateToLogin
üü #
?
üü# $
.
üü$ %
Invoke
üü% +
(
üü+ ,
)
üü, -
;
üü- .
}
†† 
else
°° 
{
¢¢ 

MessageBox
££ 
.
££ 
Show
££ #
(
££# $
result
££$ *
.
££* +
message
££+ 2
,
££2 3
Lang
££4 8
.
££8 9

ErrorTitle
££9 C
,
££C D
MessageBoxButton
££E U
.
££U V
OK
££V X
,
££X Y
MessageBoxImage
££Z i
.
££i j
Error
££j o
)
££o p
;
££p q
if
§§ 
(
§§ 
result
§§ 
.
§§ 
message
§§ &
==
§§' )
Lang
§§* .
.
§§. /4
&GlobalVerificationInvalidOrExpiredCode
§§/ U
)
§§U V
{
•• 
IsStep1Visible
¶¶ &
=
¶¶' (
false
¶¶) .
;
¶¶. /
IsStep2Visible
ßß &
=
ßß' (
true
ßß) -
;
ßß- .
IsStep3Visible
®® &
=
®®' (
false
®®) .
;
®®. /
VerificationCode
©© (
=
©©) *
string
©©+ 1
.
©©1 2
Empty
©©2 7
;
©©7 8
}
™™ 
}
´´ 
}
¨¨ 
catch
≠≠ 
(
≠≠ 
	Exception
≠≠ 
ex
≠≠ 
)
≠≠  
{
ÆÆ 

MessageBox
ØØ 
.
ØØ 
Show
ØØ 
(
ØØ  
$"
ØØ  "
$str
ØØ" )
{
ØØ) *
ex
ØØ* ,
.
ØØ, -
Message
ØØ- 4
}
ØØ4 5
"
ØØ5 6
,
ØØ6 7
Lang
ØØ8 <
.
ØØ< =

ErrorTitle
ØØ= G
,
ØØG H
MessageBoxButton
ØØI Y
.
ØØY Z
OK
ØØZ \
,
ØØ\ ]
MessageBoxImage
ØØ^ m
.
ØØm n
Error
ØØn s
)
ØØs t
;
ØØt u
}
∞∞ 
finally
±± 
{
≤≤ 
setBusy
≥≥ 
(
≥≥ 
false
≥≥ 
)
≥≥ 
;
≥≥ 
}
¥¥ 
}
µµ 	
private
∑∑ 
void
∑∑ 
executeGoBack
∑∑ "
(
∑∑" #
)
∑∑# $
{
∏∏ 	
if
ππ 
(
ππ 
IsStep3Visible
ππ 
)
ππ 
{
∫∫ 
IsStep1Visible
ªª 
=
ªª  
false
ªª! &
;
ªª& '
IsStep2Visible
ºº 
=
ºº  
true
ºº! %
;
ºº% &
IsStep3Visible
ΩΩ 
=
ΩΩ  
false
ΩΩ! &
;
ΩΩ& '
NewPassword
ææ 
=
ææ 
$str
ææ  
;
ææ  !
ConfirmPassword
øø 
=
øø  !
$str
øø" $
;
øø$ %
}
¿¿ 
else
¡¡ 
if
¡¡ 
(
¡¡ 
IsStep2Visible
¡¡ #
)
¡¡# $
{
¬¬ 
IsStep1Visible
√√ 
=
√√  
true
√√! %
;
√√% &
IsStep2Visible
ƒƒ 
=
ƒƒ  
false
ƒƒ! &
;
ƒƒ& '
IsStep3Visible
≈≈ 
=
≈≈  
false
≈≈! &
;
≈≈& '
VerificationCode
∆∆  
=
∆∆! "
string
∆∆# )
.
∆∆) *
Empty
∆∆* /
;
∆∆/ 0
}
«« 
else
»» 
{
…… 
navigateBack
   
?
   
.
   
Invoke
   $
(
  $ %
)
  % &
;
  & '
}
ÀÀ 
}
ÃÃ 	
private
ŒŒ 
void
ŒŒ $
raiseCanExecuteChanged
ŒŒ +
(
ŒŒ+ ,
)
ŒŒ, -
{
œœ 	
Application
–– 
.
–– 
Current
–– 
.
––  

Dispatcher
––  *
?
––* +
.
––+ ,
Invoke
––, 2
(
––2 3
(
––3 4
)
––4 5
=>
––6 8
{
—— 
CommandManager
““ 
.
““ (
InvalidateRequerySuggested
““ 9
(
““9 :
)
““: ;
;
““; <
OnPropertyChanged
”” !
(
””! "
nameof
””" (
(
””( )
CanSendCode
””) 4
)
””4 5
)
””5 6
;
””6 7
OnPropertyChanged
‘‘ !
(
‘‘! "
nameof
‘‘" (
(
‘‘( )
CanVerifyCode
‘‘) 6
)
‘‘6 7
)
‘‘7 8
;
‘‘8 9
OnPropertyChanged
’’ !
(
’’! "
nameof
’’" (
(
’’( )
CanResendCode
’’) 6
)
’’6 7
)
’’7 8
;
’’8 9
OnPropertyChanged
÷÷ !
(
÷÷! "
nameof
÷÷" (
(
÷÷( )
CanSavePassword
÷÷) 8
)
÷÷8 9
)
÷÷9 :
;
÷÷: ;
}
◊◊ 
)
◊◊ 
;
◊◊ 
}
ÿÿ 	
private
ŸŸ 
void
ŸŸ 
setBusy
ŸŸ 
(
ŸŸ 
bool
ŸŸ !
value
ŸŸ" '
)
ŸŸ' (
{
⁄⁄ 	
IsBusy
€€ 
=
€€ 
value
€€ 
;
€€ 
}
‹‹ 	
}
›› 
}ﬁﬁ “J
°C:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\ViewModel\Authentication\VerificationViewModel.cs
	namespace

 	
MindWeaveClient


 
.

 
	ViewModel

 #
.

# $
Authentication

$ 2
{ 
public 

class !
VerificationViewModel &
:' (
BaseViewModel) 6
{ 
private 
string 

emailValue !
;! "
private 
string !
verificationCodeValue ,
;, -
public 
string 
Email 
{ 
get !
=>" $

emailValue% /
;/ 0
set1 4
{5 6

emailValue7 A
=B C
valueD I
;I J
OnPropertyChangedK \
(\ ]
)] ^
;^ _
}` a
}b c
public 
string 
VerificationCode &
{ 	
get 
=> !
verificationCodeValue (
;( )
set 
{ !
verificationCodeValue %
=& '
value( -
;- .
OnPropertyChanged !
(! "
)" #
;# $
( 
( 
RelayCommand 
) 
VerifyCommand ,
), -
.- ."
RaiseCanExecuteChanged. D
(D E
)E F
;F G
} 
} 	
public 
ICommand 
VerifyCommand %
{& '
get( +
;+ ,
}- .
public 
ICommand 
GoBackCommand %
{& '
get( +
;+ ,
}- .
public 
ICommand 
ResendCodeCommand )
{* +
get, /
;/ 0
}1 2
private!! 
readonly!! 
Action!! 
<!!  
Page!!  $
>!!$ %

navigateTo!!& 0
;!!0 1
private"" 
readonly"" 
Action"" 
navigateBack""  ,
;"", -
public$$ !
VerificationViewModel$$ $
($$$ %
string$$% +
email$$, 1
,$$1 2
Action$$3 9
<$$9 :
Page$$: >
>$$> ?

navigateTo$$@ J
,$$J K
Action$$L R
navigateBack$$S _
)$$_ `
{%% 	
this&& 
.&& 
Email&& 
=&& 
email&& 
;&& 
this'' 
.'' 

navigateTo'' 
='' 

navigateTo'' (
;''( )
this(( 
.(( 
navigateBack(( 
=(( 
navigateBack((  ,
;((, -
VerifyCommand** 
=** 
new** 
RelayCommand**  ,
(**, -
async**- 2
(**3 4
param**4 9
)**9 :
=>**; =
await**> C
executeVerifyAsync**D V
(**V W
)**W X
,**X Y
(**Z [
param**[ `
)**` a
=>**b d
canExecuteVerify**e u
(**u v
)**v w
)**w x
;**x y
GoBackCommand++ 
=++ 
new++ 
RelayCommand++  ,
(++, -
(++- .
param++. 3
)++3 4
=>++5 7
executeGoBack++8 E
(++E F
)++F G
)++G H
;++H I
ResendCodeCommand,, 
=,, 
new,,  #
RelayCommand,,$ 0
(,,0 1
async,,1 6
(,,7 8
param,,8 =
),,= >
=>,,? A
await,,B G"
executeResendCodeAsync,,H ^
(,,^ _
),,_ `
),,` a
;,,a b
}-- 	
private// 
bool// 
canExecuteVerify// %
(//% &
)//& '
{00 	
return11 
!11 
string11 
.11 
IsNullOrWhiteSpace11 -
(11- .
VerificationCode11. >
)11> ?
&&22 
VerificationCode22 &
.22& '
Length22' -
==22. 0
$num221 2
&&33 
VerificationCode33 &
.33& '
All33' *
(33* +
char33+ /
.33/ 0
IsDigit330 7
)337 8
;338 9
}44 	
private66 
async66 
Task66 
executeVerifyAsync66 -
(66- .
)66. /
{77 	
if88 
(88 
!88 
canExecuteVerify88 !
(88! "
)88" #
)88# $
{99 

MessageBox:: 
.:: 
Show:: 
(::  

Properties::  *
.::* +
Langs::+ 0
.::0 1
Lang::1 5
.::5 6)
VerificationCodeInvalidFormat::6 S
,::S T
$str::U f
,::f g
MessageBoxButton::h x
.::x y
OK::y {
,::{ |
MessageBoxImage	::} å
.
::å ç
Warning
::ç î
)
::î ï
;
::ï ñ
return;; 
;;; 
}<< 
try>> 
{?? 
var@@ 
client@@ 
=@@ 
new@@  '
AuthenticationManagerClient@@! <
(@@< =
)@@= >
;@@> ?
OperationResultDtoAA "
resultAA# )
=AA* +
awaitAA, 1
clientAA2 8
.AA8 9
verifyAccountAsyncAA9 K
(AAK L
EmailAAL Q
,AAQ R
VerificationCodeAAS c
)AAc d
;AAd e
ifCC 
(CC 
resultCC 
.CC 
successCC "
)CC" #
{DD 

MessageBoxEE 
.EE 
ShowEE #
(EE# $
resultEE$ *
.EE* +
messageEE+ 2
,EE2 3
$strEE4 ;
,EE; <
MessageBoxButtonEE= M
.EEM N
OKEEN P
,EEP Q
MessageBoxImageEER a
.EEa b
InformationEEb m
)EEm n
;EEn o
thisFF 
.FF 

navigateToFF #
(FF# $
newFF$ '
	LoginPageFF( 1
(FF1 2
)FF2 3
)FF3 4
;FF4 5
}GG 
elseHH 
{II 

MessageBoxJJ 
.JJ 
ShowJJ #
(JJ# $
resultJJ$ *
.JJ* +
messageJJ+ 2
,JJ2 3
$strJJ4 J
,JJJ K
MessageBoxButtonJJL \
.JJ\ ]
OKJJ] _
,JJ_ `
MessageBoxImageJJa p
.JJp q
ErrorJJq v
)JJv w
;JJw x
}KK 
}LL 
catchMM 
(MM 
	ExceptionMM 
exMM 
)MM  
{NN 

MessageBoxOO 
.OO 
ShowOO 
(OO  
$"OO  "
$strOO" 4
{OO4 5
exOO5 7
.OO7 8
MessageOO8 ?
}OO? @
"OO@ A
,OOA B
$strOOC J
,OOJ K
MessageBoxButtonOOL \
.OO\ ]
OKOO] _
,OO_ `
MessageBoxImageOOa p
.OOp q
ErrorOOq v
)OOv w
;OOw x
}PP 
}QQ 	
privateSS 
voidSS 
executeGoBackSS "
(SS" #
)SS# $
{TT 	
thisUU 
.UU 
navigateBackUU 
?UU 
.UU 
InvokeUU %
(UU% &
)UU& '
;UU' (
}VV 	
privateXX 
asyncXX 
TaskXX "
executeResendCodeAsyncXX 1
(XX1 2
)XX2 3
{YY 	
tryZZ 
{[[ 
var\\ 
client\\ 
=\\ 
new\\  '
AuthenticationManagerClient\\! <
(\\< =
)\\= >
;\\> ?
OperationResultDto]] "
result]]# )
=]]* +
await]], 1
client]]2 8
.]]8 9'
resendVerificationCodeAsync]]9 T
(]]T U
Email]]U Z
)]]Z [
;]][ \
if__ 
(__ 
result__ 
.__ 
success__ "
)__" #
{`` 

MessageBoxaa 
.aa 
Showaa #
(aa# $
$straa$ >
,aa> ?
$straa@ O
,aaO P
MessageBoxButtonaaQ a
.aaa b
OKaab d
,aad e
MessageBoxImageaaf u
.aau v
Information	aav Å
)
aaÅ Ç
;
aaÇ É
}bb 
elsecc 
{dd 

MessageBoxee 
.ee 
Showee #
(ee# $
resultee$ *
.ee* +
messageee+ 2
,ee2 3
$stree4 ;
,ee; <
MessageBoxButtonee= M
.eeM N
OKeeN P
,eeP Q
MessageBoxImageeeR a
.eea b
Erroreeb g
)eeg h
;eeh i
}ff 
}gg 
catchhh 
(hh 
	Exceptionhh 
exhh 
)hh  
{ii 

MessageBoxjj 
.jj 
Showjj 
(jj  
$"jj  "
$strjj" @
{jj@ A
exjjA C
.jjC D
MessagejjD K
}jjK L
"jjL M
,jjM N
$strjjO b
,jjb c
MessageBoxButtonjjd t
.jjt u
OKjju w
,jjw x
MessageBoxImage	jjy à
.
jjà â
Error
jjâ é
)
jjé è
;
jjè ê
}kk 
}ll 	
}mm 
}nn °u
öC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\ViewModel\Authentication\LoginViewModel.cs
	namespace 	
MindWeaveClient
 
. 
	ViewModel #
.# $
Authentication$ 2
{ 
public 

class 
LoginViewModel 
:  !
BaseViewModel" /
{ 
private 
string 

emailValue !
;! "
private 
string 
passwordValue $
;$ %
private 
readonly 
Action 
<  
Page  $
>$ %

navigateTo& 0
;0 1
private 
bool '
showUnverifiedControlsValue 0
=1 2
false3 8
;8 9
public 
string 
Email 
{ 
get !
=>" $

emailValue% /
;/ 0
set1 4
{5 6

emailValue7 A
=B C
valueD I
;I J
OnPropertyChangedK \
(\ ]
)] ^
;^ _
}` a
}b c
public 
string 
Password 
{  
get! $
=>% '
passwordValue( 5
;5 6
set7 :
{; <
passwordValue= J
=K L
valueM R
;R S
OnPropertyChangedT e
(e f
)f g
;g h
}i j
}k l
public 
bool "
ShowUnverifiedControls *
{+ ,
get- 0
=>1 3'
showUnverifiedControlsValue4 O
;O P
setQ T
{U V'
showUnverifiedControlsValueW r
=s t
valueu z
;z {
OnPropertyChanged	| ç
(
ç é
)
é è
;
è ê
}
ë í
}
ì î
public 
ICommand 
LoginCommand $
{% &
get' *
;* +
}, -
public 
ICommand 
SignUpCommand %
{& '
get( +
;+ ,
}- .
public 
ICommand !
ForgotPasswordCommand -
{. /
get0 3
;3 4
}5 6
public 
ICommand 
GuestLoginCommand )
{* +
get, /
;/ 0
}1 2
public 
ICommand %
ResendVerificationCommand 1
{2 3
get4 7
;7 8
}9 :
public 
LoginViewModel 
( 
Action $
<$ %
Page% )
>) *
navigateAction+ 9
)9 :
{   	

navigateTo!! 
=!! 
navigateAction!! '
;!!' (
LoginCommand"" 
="" 
new"" 
RelayCommand"" +
(""+ ,
async"", 1
(""2 3
param""3 8
)""8 9
=>"": <
await""= B
executeLoginAsync""C T
(""T U
)""U V
,""V W
(""X Y
param""Y ^
)""^ _
=>""` b
canExecuteLogin""c r
(""r s
)""s t
)""t u
;""u v
SignUpCommand## 
=## 
new## 
RelayCommand##  ,
(##, -
(##- .
param##. 3
)##3 4
=>##5 7
executeGoToSignUp##8 I
(##I J
)##J K
)##K L
;##L M!
ForgotPasswordCommand$$ !
=$$" #
new$$$ '
RelayCommand$$( 4
($$4 5
($$5 6
param$$6 ;
)$$; <
=>$$= ?%
executeGoToForgotPassword$$@ Y
($$Y Z
)$$Z [
)$$[ \
;$$\ ]
GuestLoginCommand%% 
=%% 
new%%  #
RelayCommand%%$ 0
(%%0 1
(%%1 2
param%%2 7
)%%7 8
=>%%9 ; 
executeGoToGuestJoin%%< P
(%%P Q
)%%Q R
)%%R S
;%%S T%
ResendVerificationCommand&& %
=&&& '
new&&( +
RelayCommand&&, 8
(&&8 9
async&&9 >
(&&? @
param&&@ E
)&&E F
=>&&G I
await&&J O*
executeResendVerificationAsync&&P n
(&&n o
)&&o p
)&&p q
;&&q r
}'' 	
private)) 
bool)) 
canExecuteLogin)) $
())$ %
)))% &
{** 	
return++ 
!++ 
string++ 
.++ 
IsNullOrWhiteSpace++ -
(++- .
Email++. 3
)++3 4
&&++5 7
!++8 9
string++9 ?
.++? @
IsNullOrWhiteSpace++@ R
(++R S
Password++S [
)++[ \
;++\ ]
},, 	
private.. 
async.. 
Task.. 
executeLoginAsync.. ,
(.., -
)..- .
{// 	
setBusy00 
(00 
true00 
)00 
;00 
try11 
{22 
var33 
client33 
=33 
new33  '
AuthenticationManagerClient33! <
(33< =
)33= >
;33> ?
var44 
loginCredentials44 $
=44% &
new44' *
LoginDto44+ 3
{55 
email66 
=66 
this66  
.66  !
Email66! &
,66& '
password77 
=77 
this77 #
.77# $
Password77$ ,
}88 
;88 
LoginResultDto:: 
result:: %
=::& '
await::( -
client::. 4
.::4 5

loginAsync::5 ?
(::? @
loginCredentials::@ P
)::P Q
;::Q R
if<< 
(<< 
result<< 
.<< 
operationResult<< *
.<<* +
success<<+ 2
)<<2 3
{== 
SessionService>> "
.>>" #

SetSession>># -
(>>- .
result>>. 4
.>>4 5
username>>5 =
,>>= >
result>>? E
.>>E F

avatarPath>>F P
)>>P Q
;>>Q R
bool@@ 
socialConnected@@ (
=@@) *&
SocialServiceClientManager@@+ E
.@@E F
instance@@F N
.@@N O
Connect@@O V
(@@V W
result@@W ]
.@@] ^
username@@^ f
)@@f g
;@@g h
ifAA 
(AA 
!AA 
socialConnectedAA (
)AA( )
{BB 

MessageBoxCC "
.CC" #
ShowCC# '
(CC' (
$strCC( y
,CCy z
$str	CC{ Ñ
,
CCÑ Ö
MessageBoxButton
CCÜ ñ
.
CCñ ó
OK
CCó ô
,
CCô ö
MessageBoxImage
CCõ ™
.
CC™ ´
Warning
CC´ ≤
)
CC≤ ≥
;
CC≥ ¥
}DD +
MatchmakingServiceClientManagerFF 3
.FF3 4
instanceFF4 <
.FF< =
ConnectFF= D
(FFD E
)FFE F
;FFF G

MessageBoxHH 
.HH 
ShowHH #
(HH# $
resultHH$ *
.HH* +
operationResultHH+ :
.HH: ;
messageHH; B
,HHB C
LangHHD H
.HHH I
InfoMsgTitleSuccessHHI \
,HH\ ]
MessageBoxButtonHH^ n
.HHn o
OKHHo q
,HHq r
MessageBoxImage	HHs Ç
.
HHÇ É
Information
HHÉ é
)
HHé è
;
HHè ê
varJJ 
currentWindowJJ %
=JJ& '
ApplicationJJ( 3
.JJ3 4
CurrentJJ4 ;
.JJ; <

MainWindowJJ< F
;JJF G
varKK 
mainAppWindowKK %
=KK& '
newKK( +

MainWindowKK, 6
(KK6 7
)KK7 8
;KK8 9
mainAppWindowLL !
.LL! "
ShowLL" &
(LL& '
)LL' (
;LL( )
currentWindowMM !
?MM! "
.MM" #
CloseMM# (
(MM( )
)MM) *
;MM* +
}NN 
elseOO 
{PP 
ifQQ 
(QQ 
resultQQ 
.QQ 

resultCodeQQ )
==QQ* ,
$strQQ- C
)QQC D
{RR "
ShowUnverifiedControlsSS .
=SS/ 0
trueSS1 5
;SS5 6
}TT 
elseUU 
{VV 

MessageBoxWW "
.WW" #
ShowWW# '
(WW' (
resultWW( .
.WW. /
operationResultWW/ >
.WW> ?
messageWW? F
,WWF G
LangWWH L
.WWL M

ErrorTitleWWM W
,WWW X
MessageBoxButtonWWY i
.WWi j
OKWWj l
,WWl m
MessageBoxImageWWn }
.WW} ~
Error	WW~ É
)
WWÉ Ñ
;
WWÑ Ö
}XX 
}YY 
}ZZ 
catch[[ 
([[ 
	Exception[[ 
ex[[ 
)[[  
{\\ 
handleError]] 
(]] 
$str]] N
,]]N O
ex]]P R
)]]R S
;]]S T
}^^ 
finally__ 
{`` 
setBusyaa 
(aa 
falseaa 
)aa 
;aa 
}bb 
}cc 	
privateee 
asyncee 
Taskee *
executeResendVerificationAsyncee 9
(ee9 :
)ee: ;
{ff 	
setBusygg 
(gg 
truegg 
)gg 
;gg 
tryhh 
{ii 
varjj 
clientjj 
=jj 
newjj  '
AuthenticationManagerClientjj! <
(jj< =
)jj= >
;jj> ?
OperationResultDtokk "
resultkk# )
=kk* +
awaitkk, 1
clientkk2 8
.kk8 9'
resendVerificationCodeAsynckk9 T
(kkT U
thiskkU Y
.kkY Z
EmailkkZ _
)kk_ `
;kk` a
ifll 
(ll 
resultll 
.ll 
successll "
)ll" #
{mm 

MessageBoxnn 
.nn 
Shownn #
(nn# $
Langnn$ (
.nn( )
InfoMsgBodyCodeSentnn) <
,nn< =
Langnn> B
.nnB C
InfoMsgTitleSuccessnnC V
,nnV W
MessageBoxButtonnnX h
.nnh i
OKnni k
,nnk l
MessageBoxImagennm |
.nn| }
Information	nn} à
)
nnà â
;
nnâ ä

navigateTooo 
(oo 
newoo "
VerificationPageoo# 3
(oo3 4
thisoo4 8
.oo8 9
Emailoo9 >
)oo> ?
)oo? @
;oo@ A
}pp 
elseqq 
{qq 
handleErrorqq "
(qq" #
resultqq# )
.qq) *
messageqq* 1
,qq1 2
nullqq3 7
)qq7 8
;qq8 9
}qq: ;
}rr 
catchss 
(ss 
	Exceptionss 
exss 
)ss  
{ss! "
handleErrorss# .
(ss. /
$strss/ E
,ssE F
exssG I
)ssI J
;ssJ K
}ssL M
finallytt 
{tt 
setBusytt 
(tt 
falsett #
)tt# $
;tt$ %
}tt& '
}uu 	
privatevv 
voidvv 
executeGoToSignUpvv &
(vv& '
)vv' (
{vv) *

navigateTovv+ 5
(vv5 6
newvv6 9
CreateAccountPagevv: K
(vvK L
)vvL M
)vvM N
;vvN O
}vvP Q
privateww 
voidww %
executeGoToForgotPasswordww .
(ww. /
)ww/ 0
{ww1 2

navigateToww3 =
(ww= >
newww> A 
PasswordRecoveryPagewwB V
(wwV W
)wwW X
)wwX Y
;wwY Z
}ww[ \
privatexx 
voidxx  
executeGoToGuestJoinxx )
(xx) *
)xx* +
{yy 	

navigateTozz 
(zz 
newzz 
GuestJoinPagezz (
(zz( )
)zz) *
)zz* +
;zz+ ,
}{{ 	
private}} 
bool}} 
isBusyValue}}  
;}}  !
public~~ 
bool~~ 
IsBusy~~ 
{~~ 
get~~  
=>~~! #
isBusyValue~~$ /
;~~/ 0
private~~1 8
set~~9 <
{~~= >
isBusyValue~~? J
=~~K L
value~~M R
;~~R S
OnPropertyChanged~~T e
(~~e f
)~~f g
;~~g h-
 RaiseCanExecuteChangedOnCommands	~~i â
(
~~â ä
)
~~ä ã
;
~~ã å
}
~~ç é
}
~~è ê
private 
void 
setBusy 
( 
bool !
value" '
)' (
{) *
IsBusy+ 1
=2 3
value4 9
;9 :
}; <
private
ÄÄ 
void
ÄÄ 
handleError
ÄÄ  
(
ÄÄ  !
string
ÄÄ! '
message
ÄÄ( /
,
ÄÄ/ 0
	Exception
ÄÄ1 :
ex
ÄÄ; =
)
ÄÄ= >
{
ÅÅ 	
string
ÇÇ 
errorDetails
ÇÇ 
=
ÇÇ  !
ex
ÇÇ" $
!=
ÇÇ% '
null
ÇÇ( ,
?
ÇÇ- .
ex
ÇÇ/ 1
.
ÇÇ1 2
Message
ÇÇ2 9
:
ÇÇ: ;
$str
ÇÇ< H
;
ÇÇH I
Console
ÉÉ 
.
ÉÉ 
	WriteLine
ÉÉ 
(
ÉÉ 
$"
ÉÉ  
$str
ÉÉ  $
{
ÉÉ$ %
message
ÉÉ% ,
}
ÉÉ, -
$str
ÉÉ- /
{
ÉÉ/ 0
errorDetails
ÉÉ0 <
}
ÉÉ< =
"
ÉÉ= >
)
ÉÉ> ?
;
ÉÉ? @

MessageBox
ÑÑ 
.
ÑÑ 
Show
ÑÑ 
(
ÑÑ 
$"
ÑÑ 
{
ÑÑ 
message
ÑÑ &
}
ÑÑ& '
$str
ÑÑ' )
{
ÑÑ) *
errorDetails
ÑÑ* 6
}
ÑÑ6 7
"
ÑÑ7 8
,
ÑÑ8 9
Lang
ÑÑ: >
.
ÑÑ> ?

ErrorTitle
ÑÑ? I
,
ÑÑI J
MessageBoxButton
ÑÑK [
.
ÑÑ[ \
OK
ÑÑ\ ^
,
ÑÑ^ _
MessageBoxImage
ÑÑ` o
.
ÑÑo p
Error
ÑÑp u
)
ÑÑu v
;
ÑÑv w
}
ÖÖ 	
private
ÜÜ 
void
ÜÜ .
 RaiseCanExecuteChangedOnCommands
ÜÜ 5
(
ÜÜ5 6
)
ÜÜ6 7
{
áá 	
Application
àà 
.
àà 
Current
àà 
?
àà  
.
àà  !

Dispatcher
àà! +
?
àà+ ,
.
àà, -
Invoke
àà- 3
(
àà3 4
(
àà4 5
)
àà5 6
=>
àà7 9
CommandManager
àà: H
.
ààH I(
InvalidateRequerySuggested
ààI c
(
ààc d
)
ààd e
)
ààe f
;
ààf g
}
ââ 	
}
ää 
}ãã §É
ûC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\ViewModel\Authentication\GuestJoinViewModel.cs
	namespace 	
MindWeaveClient
 
. 
	ViewModel #
.# $
Authentication$ 2
{ 
public 

class 
GuestJoinViewModel #
:$ %
BaseViewModel& 3
{ 
private 
readonly 
Action 
<  
Page  $
>$ %

navigateTo& 0
;0 1
private 
readonly 
Action 
navigateBack  ,
;, -
private $
MatchmakingManagerClient (
matchmakingClient) :
=>; =+
MatchmakingServiceClientManager> ]
.] ^
instance^ f
.f g
proxyg l
;l m
private 
string 
lobbyCodeValue %
;% &
private 
string 
guestEmailValue &
;& '
private 
string  
desiredUsernameValue +
;+ ,
private 
bool 
isBusyValue  
;  !
public 
string 
	LobbyCode 
{ 	
get 
=> 
lobbyCodeValue !
;! "
set   
{!! 
if"" 
("" 
value"" 
!="" 
null"" !
&&""" $
!""% &
Regex""& +
.""+ ,
IsMatch"", 3
(""3 4
value""4 9
,""9 :
$str""; K
)""K L
)""L M
{## 
value$$ 
=$$ 
Regex$$ !
.$$! "
Match$$" '
($$' (
lobbyCodeValue$$( 6
??$$7 9
$str$$: <
,$$< =
$str$$> M
)$$M N
.$$N O
Value$$O T
;$$T U
}%% 
lobbyCodeValue&& 
=&&  
value&&! &
?&&& '
.&&' (
Trim&&( ,
(&&, -
)&&- .
;&&. /
OnPropertyChanged'' !
(''! "
)''" #
;''# $"
raiseCanExecuteChanged(( &
(((& '
)((' (
;((( )
})) 
}** 	
public,, 
string,, 

GuestEmail,,  
{-- 	
get.. 
=>.. 
guestEmailValue.. "
;.." #
set// 
{// 
guestEmailValue// !
=//" #
value//$ )
;//) *
OnPropertyChanged//+ <
(//< =
)//= >
;//> ?"
raiseCanExecuteChanged//@ V
(//V W
)//W X
;//X Y
}//Z [
}00 	
public22 
string22 
DesiredUsername22 %
{33 	
get44 
=>44  
desiredUsernameValue44 '
;44' (
set55 
{55  
desiredUsernameValue55 &
=55' (
value55) .
;55. /
OnPropertyChanged550 A
(55A B
)55B C
;55C D"
raiseCanExecuteChanged55E [
(55[ \
)55\ ]
;55] ^
}55_ `
}66 	
public88 
bool88 
IsBusy88 
{99 	
get:: 
=>:: 
isBusyValue:: 
;:: 
private;; 
set;; 
{;; 
setBusy;; !
(;;! "
value;;" '
);;' (
;;;( )
};;* +
}<< 	
public>> 
bool>> 
CanJoinAsGuest>> "
=>>># %
!?? 
IsBusy?? 
&&?? 
!@@ 
string@@ 
.@@ 
IsNullOrWhiteSpace@@ &
(@@& '
	LobbyCode@@' 0
)@@0 1
&&@@2 4
	LobbyCode@@5 >
.@@> ?
Length@@? E
==@@F H
$num@@I J
&&@@K M
RegexAA 
.AA 
IsMatchAA 
(AA 
	LobbyCodeAA #
,AA# $
$strAA% 7
)AA7 8
&&AA9 ;
!BB 
stringBB 
.BB 
IsNullOrWhiteSpaceBB &
(BB& '

GuestEmailBB' 1
)BB1 2
&&BB3 5
isValidEmailBB6 B
(BBB C

GuestEmailBBC M
)BBM N
&&BBO Q
!CC 
stringCC 
.CC 
IsNullOrWhiteSpaceCC &
(CC& '
DesiredUsernameCC' 6
)CC6 7
&&CC8 : 
isValidGuestUsernameCC; O
(CCO P
DesiredUsernameCCP _
)CC_ `
;CC` a
publicEE 
ICommandEE 
JoinAsGuestCommandEE *
{EE+ ,
getEE- 0
;EE0 1
}EE2 3
publicFF 
ICommandFF 
GoBackCommandFF %
{FF& '
getFF( +
;FF+ ,
}FF- .
publicHH 
GuestJoinViewModelHH !
(HH! "
ActionHH" (
<HH( )
PageHH) -
>HH- .
navigateToActionHH/ ?
,HH? @
ActionHHA G
navigateBackActionHHH Z
)HHZ [
{II 	

navigateToJJ 
=JJ 
navigateToActionJJ )
;JJ) *
navigateBackKK 
=KK 
navigateBackActionKK -
;KK- .
JoinAsGuestCommandMM 
=MM  
newMM! $
RelayCommandMM% 1
(MM1 2
asyncMM2 7
paramMM8 =
=>MM> @
awaitMMA F#
executeJoinAsGuestAsyncMMG ^
(MM^ _
)MM_ `
,MM` a
paramMMb g
=>MMh j
CanJoinAsGuestMMk y
)MMy z
;MMz {
GoBackCommandNN 
=NN 
newNN 
RelayCommandNN  ,
(NN, -
paramNN- 2
=>NN3 5
navigateBackNN6 B
?NNB C
.NNC D
InvokeNND J
(NNJ K
)NNK L
,NNL M
paramNNN S
=>NNT V
!NNW X
IsBusyNNX ^
)NN^ _
;NN_ `
}OO 	
privateQQ 
asyncQQ 
TaskQQ #
executeJoinAsGuestAsyncQQ 2
(QQ2 3
)QQ3 4
{RR 	
ifSS 
(SS 
!SS 
CanJoinAsGuestSS 
)SS  
returnSS! '
;SS' (
ifUU 
(UU 
!UU +
MatchmakingServiceClientManagerUU 0
.UU0 1
instanceUU1 9
.UU9 :
EnsureConnectedUU: I
(UUI J
)UUJ K
)UUK L
{VV 

MessageBoxWW 
.WW 
ShowWW 
(WW  
LangWW  $
.WW$ %$
CannotConnectMatchmakingWW% =
,WW= >
LangWW? C
.WWC D

ErrorTitleWWD N
,WWN O
MessageBoxButtonWWP `
.WW` a
OKWWa c
,WWc d
MessageBoxImageWWe t
.WWt u
WarningWWu |
)WW| }
;WW} ~
returnXX 
;XX 
}YY 
setBusy[[ 
([[ 
true[[ 
)[[ 
;[[ 
var]] 
joinRequest]] 
=]] 
new]] !
GuestJoinRequestDto]]" 5
{^^ 
	lobbyCode__ 
=__ 
this__  
.__  !
	LobbyCode__! *
,__* +

guestEmail`` 
=`` 
this`` !
.``! "

GuestEmail``" ,
.``, -
Trim``- 1
(``1 2
)``2 3
,``3 4 
desiredGuestUsernameaa $
=aa% &
thisaa' +
.aa+ ,
DesiredUsernameaa, ;
.aa; <
Trimaa< @
(aa@ A
)aaA B
}bb 
;bb 
trydd 
{ee 
GuestJoinResultDtoff "
resultff# )
=ff* +
awaitff, 1
matchmakingClientff2 C
.ffC D!
joinLobbyAsGuestAsyncffD Y
(ffY Z
joinRequestffZ e
)ffe f
;fff g
ifhh 
(hh 
resulthh 
.hh 
successhh "
&&hh# %
resulthh& ,
.hh, -
initialLobbyStatehh- >
!=hh? A
nullhhB F
)hhF G
{ii 
SessionServicejj "
.jj" #

SetSessionjj# -
(jj- .
resultjj. 4
.jj4 5!
assignedGuestUsernamejj5 J
,jjJ K
nulljjL P
,jjP Q
truejjR V
)jjV W
;jjW X
ifll 
(ll 
!ll &
SocialServiceClientManagerll 3
.ll3 4
instancell4 <
.ll< =
Connectll= D
(llD E
resultllE K
.llK L!
assignedGuestUsernamellL a
)lla b
)llb c
{mm 

MessageBoxnn "
.nn" #
Shownn# '
(nn' (
$strnn( n
,nnn o
$strnnp y
,nny z
MessageBoxButton	nn{ ã
.
nnã å
OK
nnå é
,
nné è
MessageBoxImage
nnê ü
.
nnü †
Warning
nn† ß
)
nnß ®
;
nn® ©
}oo 

MessageBoxqq 
.qq 
Showqq #
(qq# $
resultqq$ *
.qq* +
messageqq+ 2
,qq2 3
Langqq4 8
.qq8 9
InfoMsgTitleSuccessqq9 L
,qqL M
MessageBoxButtonqqN ^
.qq^ _
OKqq_ a
,qqa b
MessageBoxImageqqc r
.qqr s
Informationqqs ~
)qq~ 
;	qq Ä
varss 
currentWindowss %
=ss& '
Applicationss( 3
.ss3 4
Currentss4 ;
.ss; <
Windowsss< C
.ssC D
OfTypessD J
<ssJ K 
AuthenticationWindowssK _
>ss_ `
(ss` a
)ssa b
.ssb c
FirstOrDefaultssc q
(ssq r
)ssr s
;sss t
vartt 
mainAppWindowtt %
=tt& '
newtt( +

MainWindowtt, 6
(tt6 7
)tt7 8
;tt8 9
varvv 
	lobbyPagevv !
=vv" #
newvv$ '
	LobbyPagevv( 1
(vv1 2
)vv2 3
;vv3 4
	lobbyPageww 
.ww 
DataContextww )
=ww* +
newww, /
LobbyViewModelww0 >
(ww> ?
resultxx 
.xx 
initialLobbyStatexx 0
,xx0 1
pageyy 
=>yy 
mainAppWindowyy  -
.yy- .
	MainFrameyy. 7
.yy7 8
Navigateyy8 @
(yy@ A
pageyyA E
)yyE F
,yyF G
(zz 
)zz 
=>zz 
mainAppWindowzz +
.zz+ ,
	MainFramezz, 5
.zz5 6
Navigatezz6 >
(zz> ?
newzz? B
MainMenuPagezzC O
(zzO P
pagezzP T
=>zzU W
mainAppWindowzzX e
.zze f
	MainFramezzf o
.zzo p
Navigatezzp x
(zzx y
pagezzy }
)zz} ~
)zz~ 
)	zz Ä
){{ 
;{{ 
mainAppWindow|| !
.||! "
	MainFrame||" +
.||+ ,
Navigate||, 4
(||4 5
	lobbyPage||5 >
)||> ?
;||? @
mainAppWindow~~ !
.~~! "
Show~~" &
(~~& '
)~~' (
;~~( )
currentWindow !
?! "
." #
Close# (
(( )
)) *
;* +
}
ÄÄ 
else
ÅÅ 
{
ÇÇ 

MessageBox
ÉÉ 
.
ÉÉ 
Show
ÉÉ #
(
ÉÉ# $
result
ÉÉ$ *
.
ÉÉ* +
message
ÉÉ+ 2
??
ÉÉ3 5
$str
ÉÉ6 H
,
ÉÉH I
Lang
ÉÉJ N
.
ÉÉN O

ErrorTitle
ÉÉO Y
,
ÉÉY Z
MessageBoxButton
ÉÉ[ k
.
ÉÉk l
OK
ÉÉl n
,
ÉÉn o
MessageBoxImage
ÉÉp 
.ÉÉ Ä
ErrorÉÉÄ Ö
)ÉÉÖ Ü
;ÉÉÜ á
}
ÑÑ 
}
ÖÖ 
catch
ÜÜ 
(
ÜÜ 
	Exception
ÜÜ 
ex
ÜÜ 
)
ÜÜ  
{
áá 
handleError
àà 
(
àà 
$str
àà N
,
ààN O
ex
ààP R
)
ààR S
;
ààS T-
MatchmakingServiceClientManager
ââ /
.
ââ/ 0
instance
ââ0 8
.
ââ8 9

Disconnect
ââ9 C
(
ââC D
)
ââD E
;
ââE F
}
ää 
finally
ãã 
{
åå 
setBusy
çç 
(
çç 
false
çç 
)
çç 
;
çç 
}
éé 
}
èè 	
private
ëë 
void
ëë 
setBusy
ëë 
(
ëë 
bool
ëë !
value
ëë" '
)
ëë' (
{
íí 	
isBusyValue
ìì 
=
ìì 
value
ìì 
;
ìì  
OnPropertyChanged
îî 
(
îî 
nameof
îî $
(
îî$ %
IsBusy
îî% +
)
îî+ ,
)
îî, -
;
îî- .$
raiseCanExecuteChanged
ïï "
(
ïï" #
)
ïï# $
;
ïï$ %
}
ññ 	
private
òò 
void
òò $
raiseCanExecuteChanged
òò +
(
òò+ ,
)
òò, -
{
ôô 	
OnPropertyChanged
öö 
(
öö 
nameof
öö $
(
öö$ %
CanJoinAsGuest
öö% 3
)
öö3 4
)
öö4 5
;
öö5 6
Application
õõ 
.
õõ 
Current
õõ 
?
õõ  
.
õõ  !

Dispatcher
õõ! +
?
õõ+ ,
.
õõ, -
Invoke
õõ- 3
(
õõ3 4
(
õõ4 5
)
õõ5 6
=>
õõ7 9
CommandManager
õõ: H
.
õõH I(
InvalidateRequerySuggested
õõI c
(
õõc d
)
õõd e
)
õõe f
;
õõf g
}
úú 	
private
ûû 
void
ûû 
handleError
ûû  
(
ûû  !
string
ûû! '
message
ûû( /
,
ûû/ 0
	Exception
ûû1 :
ex
ûû; =
)
ûû= >
{
üü 	
string
†† 
errorDetails
†† 
=
††  !
ex
††" $
?
††$ %
.
††% &
Message
††& -
??
††. 0
$str
††1 G
;
††G H
Console
°° 
.
°° 
	WriteLine
°° 
(
°° 
$"
°°  
$str
°°  $
{
°°$ %
message
°°% ,
}
°°, -
$str
°°- /
{
°°/ 0
errorDetails
°°0 <
}
°°< =
"
°°= >
)
°°> ?
;
°°? @

MessageBox
¢¢ 
.
¢¢ 
Show
¢¢ 
(
¢¢ 
$"
¢¢ 
{
¢¢ 
message
¢¢ &
}
¢¢& '
$str
¢¢' *
{
¢¢* +
errorDetails
¢¢+ 7
}
¢¢7 8
"
¢¢8 9
,
¢¢9 :
Lang
¢¢; ?
.
¢¢? @

ErrorTitle
¢¢@ J
,
¢¢J K
MessageBoxButton
¢¢L \
.
¢¢\ ]
OK
¢¢] _
,
¢¢_ `
MessageBoxImage
¢¢a p
.
¢¢p q
Error
¢¢q v
)
¢¢v w
;
¢¢w x
}
££ 	
private
§§ 
bool
§§ 
isValidEmail
§§ !
(
§§! "
string
§§" (
email
§§) .
)
§§. /
{
•• 	
if
¶¶ 
(
¶¶ 
string
¶¶ 
.
¶¶  
IsNullOrWhiteSpace
¶¶ )
(
¶¶) *
email
¶¶* /
)
¶¶/ 0
)
¶¶0 1
return
¶¶2 8
false
¶¶9 >
;
¶¶> ?
try
ßß 
{
®® 
var
©© 
addr
©© 
=
©© 
new
©© 
System
©© %
.
©©% &
Net
©©& )
.
©©) *
Mail
©©* .
.
©©. /
MailAddress
©©/ :
(
©©: ;
email
©©; @
)
©©@ A
;
©©A B
return
™™ 
addr
™™ 
.
™™ 
Address
™™ #
==
™™$ &
email
™™' ,
;
™™, -
}
´´ 
catch
¨¨ 
{
≠≠ 
return
ÆÆ 
false
ÆÆ 
;
ÆÆ 
}
ØØ 
}
∞∞ 	
private
≤≤ 
bool
≤≤ "
isValidGuestUsername
≤≤ )
(
≤≤) *
string
≤≤* 0
username
≤≤1 9
)
≤≤9 :
{
≥≥ 	
if
¥¥ 
(
¥¥ 
string
¥¥ 
.
¥¥  
IsNullOrWhiteSpace
¥¥ )
(
¥¥) *
username
¥¥* 2
)
¥¥2 3
)
¥¥3 4
return
¥¥5 ;
false
¥¥< A
;
¥¥A B
return
µµ 
Regex
µµ 
.
µµ 
IsMatch
µµ  
(
µµ  !
username
µµ! )
,
µµ) *
$str
µµ+ @
)
µµ@ A
;
µµA B
}
∂∂ 	
}
∑∑ 
}∏∏ »W
¢C:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\ViewModel\Authentication\CreateAccountViewModel.cs
	namespace		 	
MindWeaveClient		
 
.		 
	ViewModel		 #
.		# $
Authentication		$ 2
{

 
public 

class "
CreateAccountViewModel '
:( )
BaseViewModel* 7
{ 
private 
string 
firstNameValue %
;% &
private 
string 
lastNameValue $
;$ %
private 
string 
usernameValue $
;$ %
private 
string 

emailValue !
;! "
private 
DateTime 
? 
birthDateValue (
=) *
DateTime+ 3
.3 4
Now4 7
;7 8
private 
string 
passwordValue $
;$ %
public 
string 
	FirstName 
{  !
get" %
=>& (
firstNameValue) 7
;7 8
set9 <
{= >
firstNameValue? M
=N O
valueP U
;U V
OnPropertyChangedW h
(h i
)i j
;j k
}l m
}n o
public 
string 
LastName 
{  
get! $
=>% '
lastNameValue( 5
;5 6
set7 :
{; <
lastNameValue= J
=K L
valueM R
;R S
OnPropertyChangedT e
(e f
)f g
;g h
}i j
}k l
public 
string 
Username 
{  
get! $
=>% '
usernameValue( 5
;5 6
set7 :
{; <
usernameValue= J
=K L
valueM R
;R S
OnPropertyChangedT e
(e f
)f g
;g h
}i j
}k l
public 
string 
Email 
{ 
get !
=>" $

emailValue% /
;/ 0
set1 4
{5 6

emailValue7 A
=B C
valueD I
;I J
OnPropertyChangedK \
(\ ]
)] ^
;^ _
}` a
}b c
public 
DateTime 
? 
	BirthDate "
{# $
get% (
=>) +
birthDateValue, :
;: ;
set< ?
{@ A
birthDateValueB P
=Q R
valueS X
;X Y
OnPropertyChangedZ k
(k l
)l m
;m n
}o p
}q r
public 
string 
Password 
{  
get! $
=>% '
passwordValue( 5
;5 6
set7 :
{; <
passwordValue= J
=K L
valueM R
;R S
OnPropertyChangedT e
(e f
)f g
;g h
}i j
}k l
private 
bool 
isFemaleValue "
;" #
private 
bool 
isMaleValue  
;  !
private 
bool 
isOtherValue !
;! "
private 
bool !
isPreferNotToSayValue *
;* +
public!! 
bool!! 
IsFemale!! 
{!! 
get!! "
=>!!# %
isFemaleValue!!& 3
;!!3 4
set!!5 8
{!!9 :
isFemaleValue!!; H
=!!I J
value!!K P
;!!P Q
OnPropertyChanged!!R c
(!!c d
)!!d e
;!!e f
}!!g h
}!!i j
public"" 
bool"" 
IsMale"" 
{"" 
get""  
=>""! #
isMaleValue""$ /
;""/ 0
set""1 4
{""5 6
isMaleValue""7 B
=""C D
value""E J
;""J K
OnPropertyChanged""L ]
(""] ^
)""^ _
;""_ `
}""a b
}""c d
public## 
bool## 
IsOther## 
{## 
get## !
=>##" $
isOtherValue##% 1
;##1 2
set##3 6
{##7 8
isOtherValue##9 E
=##F G
value##H M
;##M N
OnPropertyChanged##O `
(##` a
)##a b
;##b c
}##d e
}##f g
public$$ 
bool$$ 
IsPreferNotToSay$$ $
{$$% &
get$$' *
=>$$+ -!
isPreferNotToSayValue$$. C
;$$C D
set$$E H
{$$I J!
isPreferNotToSayValue$$K `
=$$a b
value$$c h
;$$h i
OnPropertyChanged$$j {
($${ |
)$$| }
;$$} ~
}	$$ Ä
}
$$Å Ç
public&& 
ICommand&& 
SignUpCommand&& %
{&&& '
get&&( +
;&&+ ,
}&&- .
public'' 
ICommand'' 
GoToLoginCommand'' (
{'') *
get''+ .
;''. /
}''0 1
private)) 
readonly)) 
Action)) 
<))  
Page))  $
>))$ %

navigateTo))& 0
;))0 1
public++ "
CreateAccountViewModel++ %
(++% &
Action++& ,
<++, -
Page++- 1
>++1 2
navigateAction++3 A
)++A B
{,, 	

navigateTo-- 
=-- 
navigateAction-- '
;--' (
SignUpCommand.. 
=.. 
new.. 
RelayCommand..  ,
(.., -
async..- 2
(..3 4
param..4 9
)..9 :
=>..; =
await..> C
executeSignUp..D Q
(..Q R
)..R S
)..S T
;..T U
GoToLoginCommand// 
=// 
new// "
RelayCommand//# /
(/// 0
(//0 1
param//1 6
)//6 7
=>//8 :
executeGoToLogin//; K
(//K L
)//L M
)//M N
;//N O
}00 	
private22 
async22 
Task22 
executeSignUp22 (
(22( )
)22) *
{33 	
if44 
(44 
string44 
.44 
IsNullOrWhiteSpace44 )
(44) *
	FirstName44* 3
)443 4
||445 7
string448 >
.44> ?
IsNullOrWhiteSpace44? Q
(44Q R
Username44R Z
)44Z [
||44\ ^
string44_ e
.44e f
IsNullOrWhiteSpace44f x
(44x y
Email44y ~
)44~ 
||
44Ä Ç
string
44É â
.
44â ä 
IsNullOrWhiteSpace
44ä ú
(
44ú ù
Password
44ù •
)
44• ¶
||
44ß ©
	BirthDate
44™ ≥
==
44¥ ∂
null
44∑ ª
)
44ª º
{55 

MessageBox66 
.66 
Show66 
(66  
$str66  B
,66B C
$str66D U
,66U V
MessageBoxButton66W g
.66g h
OK66h j
,66j k
MessageBoxImage66l {
.66{ |
Warning	66| É
)
66É Ñ
;
66Ñ Ö
return77 
;77 
}88 
var:: 
userProfile:: 
=:: 
new:: !
UserProfileDto::" 0
{;; 
	firstName<< 
=<< 
this<<  
.<<  !
	FirstName<<! *
,<<* +
lastName== 
=== 
this== 
.==  
LastName==  (
,==( )
username>> 
=>> 
this>> 
.>>  
Username>>  (
,>>( )
email?? 
=?? 
this?? 
.?? 
Email?? "
,??" #
dateOfBirth@@ 
=@@ 
this@@ "
.@@" #
	BirthDate@@# ,
.@@, -
Value@@- 2
,@@2 3
genderIdAA 
=AA 
getSelectedGenderIdAA .
(AA. /
)AA/ 0
}BB 
;BB 
tryDD 
{EE 
varFF 
clientFF 
=FF 
newFF  '
AuthenticationManagerClientFF! <
(FF< =
)FF= >
;FF> ?
OperationResultDtoGG "
resultGG# )
=GG* +
awaitGG, 1
clientGG2 8
.GG8 9
registerAsyncGG9 F
(GGF G
userProfileGGG R
,GGR S
thisGGT X
.GGX Y
PasswordGGY a
)GGa b
;GGb c
ifII 
(II 
resultII 
.II 
successII "
)II" #
{JJ 

MessageBoxKK 
.KK 
ShowKK #
(KK# $
resultKK$ *
.KK* +
messageKK+ 2
,KK2 3
$strKK4 J
,KKJ K
MessageBoxButtonKKL \
.KK\ ]
OKKK] _
,KK_ `
MessageBoxImageKKa p
.KKp q
InformationKKq |
)KK| }
;KK} ~

navigateToLL 
(LL 
newLL "
VerificationPageLL# 3
(LL3 4
EmailLL4 9
)LL9 :
)LL: ;
;LL; <
}MM 
elseNN 
{OO 

MessageBoxPP 
.PP 
ShowPP #
(PP# $
resultPP$ *
.PP* +
messagePP+ 2
,PP2 3
$strPP4 I
,PPI J
MessageBoxButtonPPK [
.PP[ \
OKPP\ ^
,PP^ _
MessageBoxImagePP` o
.PPo p
ErrorPPp u
)PPu v
;PPv w
}QQ 
}RR 
catchSS 
(SS 
	ExceptionSS 
exSS 
)SS  
{TT 

MessageBoxUU 
.UU 
ShowUU 
(UU  
$"UU  "
$strUU" 5
{UU5 6
exUU6 8
.UU8 9
MessageUU9 @
}UU@ A
"UUA B
,UUB C
$strUUD K
,UUK L
MessageBoxButtonUUM ]
.UU] ^
OKUU^ `
,UU` a
MessageBoxImageUUb q
.UUq r
ErrorUUr w
)UUw x
;UUx y
}VV 
}WW 	
privateYY 
voidYY 
executeGoToLoginYY %
(YY% &
)YY& '
{ZZ 	

navigateTo[[ 
([[ 
new[[ 
	LoginPage[[ $
([[$ %
)[[% &
)[[& '
;[[' (
}\\ 	
private^^ 
int^^ 
getSelectedGenderId^^ '
(^^' (
)^^( )
{__ 	
if`` 
(`` 
IsFemale`` 
)`` 
return``  
$num``! "
;``" #
ifaa 
(aa 
IsMaleaa 
)aa 
returnaa 
$numaa  
;aa  !
ifbb 
(bb 
IsOtherbb 
)bb 
returnbb 
$numbb  !
;bb! "
returncc 
$numcc 
;cc 
}dd 	
}ee 
}ff £@
õC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Services\MatchmakingServiceClientManager.cs
	namespace 	
MindWeaveClient
 
. 
Services "
{ 
public 

sealed 
class +
MatchmakingServiceClientManager 7
{ 
private		 
static		 
readonly		 
Lazy		  $
<		$ %+
MatchmakingServiceClientManager		% D
>		D E
lazy		F J
=		K L
new

 
Lazy

 
<

 +
MatchmakingServiceClientManager

 4
>

4 5
(

5 6
(

6 7
)

7 8
=>

9 ;
new

< ?+
MatchmakingServiceClientManager

@ _
(

_ `
)

` a
)

a b
;

b c
public 
static +
MatchmakingServiceClientManager 5
instance6 >
{? @
getA D
{E F
returnG M
lazyN R
.R S
ValueS X
;X Y
}Z [
}\ ]
public $
MatchmakingManagerClient '
proxy( -
{. /
get0 3
;3 4
private5 <
set= @
;@ A
}B C
public &
MatchmakingCallbackHandler )
callbackHandler* 9
{: ;
get< ?
;? @
privateA H
setI L
;L M
}N O
private 
InstanceContext 
site  $
;$ %
private +
MatchmakingServiceClientManager /
(/ 0
)0 1
{ 	
} 	
public 
bool 
Connect 
( 
) 
{ 	
try 
{ 
if 
( 
proxy 
!= 
null !
&&" $
(% &
proxy& +
.+ ,
State, 1
==2 4
CommunicationState5 G
.G H
OpenedH N
||O Q
proxyR W
.W X
StateX ]
==^ `
CommunicationStatea s
.s t
Openingt {
){ |
)| }
{ 
return 
true 
;  
} 
if   
(   
proxy   
!=   
null   !
)  ! "
{!! 

Disconnect"" 
("" 
)""  
;""  !
}## 
callbackHandler%% 
=%%  !
new%%" %&
MatchmakingCallbackHandler%%& @
(%%@ A
)%%A B
;%%B C
site&& 
=&& 
new&& 
InstanceContext&& *
(&&* +
callbackHandler&&+ :
)&&: ;
;&&; <
proxy(( 
=(( 
new(( $
MatchmakingManagerClient(( 4
(((4 5
site((5 9
,((9 :
$str((; ^
)((^ _
;((_ `
proxy** 
.** 
Open** 
(** 
)** 
;** 
Console++ 
.++ 
	WriteLine++ !
(++! "
$str++" T
)++T U
;++U V
return,, 
true,, 
;,, 
}-- 
catch.. 
(.. 
	Exception.. 
ex.. 
)..  
{// 
Console00 
.00 
	WriteLine00 !
(00! "
$"00" $
$str00$ J
{00J K
ex00K M
.00M N
Message00N U
}00U V
"00V W
)00W X
;00X Y

Disconnect11 
(11 
)11 
;11 
return22 
false22 
;22 
}33 
}44 	
public66 
void66 

Disconnect66 
(66 
)66  
{77 	
Console88 
.88 
	WriteLine88 
(88 
$"88  
$str88  R
{88R S
proxy88S X
?88X Y
.88Y Z
State88Z _
}88_ `
"88` a
)88a b
;88b c
try99 
{:: 
if;; 
(;; 
proxy;; 
!=;; 
null;; !
);;! "
{<< 
if== 
(== 
proxy== 
.== 
State== #
====$ &
CommunicationState==' 9
.==9 :
Opened==: @
||==A C
proxy==D I
.==I J
State==J O
====P R
CommunicationState==S e
.==e f
Opening==f m
)==m n
{>> 
proxy?? 
.?? 
Close?? #
(??# $
)??$ %
;??% &
Console@@ 
.@@  
	WriteLine@@  )
(@@) *
$str@@* G
)@@G H
;@@H I
}AA 
elseBB 
ifBB 
(BB 
proxyBB "
.BB" #
StateBB# (
!=BB) +
CommunicationStateBB, >
.BB> ?
ClosedBB? E
)BBE F
{CC 
proxyDD 
.DD 
AbortDD #
(DD# $
)DD$ %
;DD% &
ConsoleEE 
.EE  
	WriteLineEE  )
(EE) *
$"EE* ,
$strEE, S
{EES T
proxyEET Y
.EEY Z
StateEEZ _
}EE_ `
$strEE` a
"EEa b
)EEb c
;EEc d
}FF 
}GG 
}HH 
catchII 
(II 
	ExceptionII 
exII 
)II  
{JJ 
ConsoleKK 
.KK 
	WriteLineKK !
(KK! "
$"KK" $
$strKK$ U
{KKU V
exKKV X
.KKX Y
MessageKKY `
}KK` a
$strKKa l
"KKl m
)KKm n
;KKn o
proxyLL 
?LL 
.LL 
AbortLL 
(LL 
)LL 
;LL 
}MM 
finallyNN 
{OO 
proxyPP 
=PP 
nullPP 
;PP 
siteQQ 
=QQ 
nullQQ 
;QQ 
callbackHandlerRR 
=RR  !
nullRR" &
;RR& '
ConsoleSS 
.SS 
	WriteLineSS !
(SS! "
$strSS" M
)SSM N
;SSN O
}TT 
}UU 	
publicWW 
boolWW 
EnsureConnectedWW #
(WW# $
)WW$ %
{XX 	
ifYY 
(YY 
proxyYY 
==YY 
nullYY 
||YY  
proxyYY! &
.YY& '
StateYY' ,
==YY- /
CommunicationStateYY0 B
.YYB C
ClosedYYC I
||YYJ L
proxyYYM R
.YYR S
StateYYS X
==YYY [
CommunicationStateYY\ n
.YYn o
FaultedYYo v
)YYv w
{ZZ 
Console[[ 
.[[ 
	WriteLine[[ !
([[! "
$"[[" $
$str[[$ ]
{[[] ^
proxy[[^ c
?[[c d
.[[d e
State[[e j
}[[j k
$str	[[k É
"
[[É Ñ
)
[[Ñ Ö
;
[[Ö Ü
return\\ 
Connect\\ 
(\\ 
)\\  
;\\  !
}]] 
if^^ 
(^^ 
proxy^^ 
.^^ 
State^^ 
==^^ 
CommunicationState^^ 1
.^^1 2
Opening^^2 9
||^^: <
proxy^^= B
.^^B C
State^^C H
==^^I K
CommunicationState^^L ^
.^^^ _
Created^^_ f
)^^f g
{__ 
Console`` 
.`` 
	WriteLine`` !
(``! "
$"``" $
$str``$ U
{``U V
proxy``V [
.``[ \
State``\ a
}``a b
$str``b r
"``r s
)``s t
;``t u
returnaa 
falseaa 
;aa 
}bb 
returncc 
proxycc 
.cc 
Statecc 
==cc !
CommunicationStatecc" 4
.cc4 5
Openedcc5 ;
;cc; <
}dd 	
}ee 
}ff ≈«
âC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Utilities\AudioManager.cs
	namespace		 	
MindWeaveClient		
 
.		 
	Utilities		 #
{

 
internal 
static 
class 
AudioManager &
{ 
private 
static 
MediaPlayer "
musicPlayer# .
=/ 0
new1 4
MediaPlayer5 @
(@ A
)A B
;B C
private 
static 
MediaPlayer "
	sfxPlayer# ,
=- .
new/ 2
MediaPlayer3 >
(> ?
)? @
;@ A
private 
static 
bool 
isMusicLoaded )
;) *
private 
static 
string 
tempMusicFilePath /
;/ 0
public 
static 
void 

Initialize %
(% &
)& '
{ 	
} 	
static 
AudioManager 
( 
) 
{ 	
loadInitialVolumes 
( 
)  
;  !
try 
{ 
string 
resourcePath #
=$ %
$str& M
;M N
Uri 
resourceUri 
=  !
new" %
Uri& )
() *
resourcePath* 6
,6 7
UriKind8 ?
.? @
Relative@ H
)H I
;I J
var 
resourceInfo  
=! "
Application# .
.. /
GetResourceStream/ @
(@ A
resourceUriA L
)L M
;M N
if   
(   
resourceInfo    
!=  ! #
null  $ (
)  ( )
{!! 
tempMusicFilePath## %
=##& '
Path##( ,
.##, -
Combine##- 4
(##4 5
Path##5 9
.##9 :
GetTempPath##: E
(##E F
)##F G
,##G H
$"##I K
$str##K U
{##U V
Guid##V Z
.##Z [
NewGuid##[ b
(##b c
)##c d
}##d e
$str##e i
"##i j
)##j k
;##k l
Debug$$ 
.$$ 
	WriteLine$$ #
($$# $
$"$$$ &
$str$$& V
{$$V W
tempMusicFilePath$$W h
}$$h i
"$$i j
)$$j k
;$$k l
using%% 
(%% 
Stream%% !
resourceStream%%" 0
=%%1 2
resourceInfo%%3 ?
.%%? @
Stream%%@ F
)%%F G
using&& 
(&& 

FileStream&& %

fileStream&&& 0
=&&1 2
new&&3 6

FileStream&&7 A
(&&A B
tempMusicFilePath&&B S
,&&S T
FileMode&&U ]
.&&] ^
Create&&^ d
,&&d e

FileAccess&&f p
.&&p q
Write&&q v
)&&v w
)&&w x
{'' 
resourceStream(( &
.((& '
CopyTo((' -
(((- .

fileStream((. 8
)((8 9
;((9 :
})) 
Debug** 
.** 
	WriteLine** #
(**# $
$"**$ &
$str**& j
"**j k
)**k l
;**l m
Uri,, 
fileUri,, 
=,,  !
new,," %
Uri,,& )
(,,) *
tempMusicFilePath,,* ;
,,,; <
UriKind,,= D
.,,D E
Absolute,,E M
),,M N
;,,N O
musicPlayer// 
.//  
MediaOpened//  +
+=//, .
(/// 0
sender//0 6
,//6 7
e//8 9
)//9 :
=>//; =
{00 
Debug11 
.11 
	WriteLine11 '
(11' (
$"11( *
$str11* f
"11f g
)11g h
;11h i
Debug22 
.22 
	WriteLine22 '
(22' (
$"22( *
$str22* f
"22f g
)22g h
;22h i
isMusicLoaded33 %
=33& '
true33( ,
;33, -
Debug44 
.44 
	WriteLine44 '
(44' (
$"44( *
$str44* d
{44d e
tempMusicFilePath44e v
}44v w
"44w x
)44x y
;44y z
Debug55 
.55 
	WriteLine55 '
(55' (
$"55( *
$str55* \
{55\ ]
musicPlayer55] h
.55h i
Volume55i o
}55o p
"55p q
)55q r
;55r s
Application88 #
.88# $
Current88$ +
.88+ ,

Dispatcher88, 6
.886 7
BeginInvoke887 B
(88B C
DispatcherPriority88C U
.88U V
Loaded88V \
,88\ ]
new88^ a
Action88b h
(88h i
(88i j
)88j k
=>88l n
{99 
Debug:: !
.::! "
	WriteLine::" +
(::+ ,
$"::, .
$str::. [
"::[ \
)::\ ]
;::] ^
try;; 
{<< 
if==  "
(==# $
isMusicLoaded==$ 1
&&==2 4
musicPlayer==5 @
.==@ A
Source==A G
!===H J
null==K O
&&==P R
musicPlayer==S ^
.==^ _
Volume==_ e
>==f g
$num==h i
)==i j
{>>  !
Debug??$ )
.??) *
	WriteLine??* 3
(??3 4
$"??4 6
$str??6 o
{??o p
musicPlayer??p {
.??{ |
Volume	??| Ç
}
??Ç É
"
??É Ñ
)
??Ñ Ö
;
??Ö Ü
musicPlayer@@$ /
.@@/ 0
Play@@0 4
(@@4 5
)@@5 6
;@@6 7
DebugAA$ )
.AA) *
	WriteLineAA* 3
(AA3 4
$"AA4 6
$strAA6 r
"AAr s
)AAs t
;AAt u
DispatcherTimerCC$ 3

checkTimerCC4 >
=CC? @
newCCA D
DispatcherTimerCCE T
{CCU V
IntervalCCW _
=CC` a
TimeSpanCCb j
.CCj k
FromMillisecondsCCk {
(CC{ |
$numCC| 
)	CC Ä
}
CCÅ Ç
;
CCÇ É

checkTimerDD$ .
.DD. /
TickDD/ 3
+=DD4 6
(DD7 8
sDD8 9
,DD9 :
argsDD; ?
)DD? @
=>DDA C
{DDD E

checkTimerEE( 2
.EE2 3
StopEE3 7
(EE7 8
)EE8 9
;EE9 :
DebugFF( -
.FF- .
	WriteLineFF. 7
(FF7 8
$"FF8 :
$strFF: s
{FFs t
musicPlayerFFt 
.	FF Ä
HasAudio
FFÄ à
}
FFà â
$str
FFâ ï
{
FFï ñ
musicPlayer
FFñ °
.
FF° ¢
Position
FF¢ ™
}
FF™ ´
"
FF´ ¨
)
FF¨ ≠
;
FF≠ Æ
}GG$ %
;GG% &

checkTimerHH$ .
.HH. /
StartHH/ 4
(HH4 5
)HH5 6
;HH6 7
}II  !
elseJJ  $
{KK  !
DebugLL$ )
.LL) *
	WriteLineLL* 3
(LL3 4
$"LL4 6
$strLL6 u
{LLu v
isMusicLoaded	LLv É
}
LLÉ Ñ
$str
LLÑ ë
{
LLë í
musicPlayer
LLí ù
.
LLù û
Source
LLû §
==
LL• ß
null
LL® ¨
}
LL¨ ≠
$str
LL≠ ∂
{
LL∂ ∑
musicPlayer
LL∑ ¬
.
LL¬ √
Volume
LL√ …
}
LL…  
"
LL  À
)
LLÀ Ã
;
LLÃ Õ
}MM  !
}NN 
catchOO !
(OO" #
	ExceptionOO# ,
playExOO- 3
)OO3 4
{PP 
DebugQQ  %
.QQ% &
	WriteLineQQ& /
(QQ/ 0
$"QQ0 2
$strQQ2 u
{QQu v
playExQQv |
}QQ| }
"QQ} ~
)QQ~ 
;	QQ Ä
}RR 
DebugSS !
.SS! "
	WriteLineSS" +
(SS+ ,
$"SS, .
$strSS. \
"SS\ ]
)SS] ^
;SS^ _
DebugTT !
.TT! "
	WriteLineTT" +
(TT+ ,
$"TT, .
$strTT. j
"TTj k
)TTk l
;TTl m
DebugUU !
.UU! "
	WriteLineUU" +
(UU+ ,
$"UU, .
$strUU. j
"UUj k
)UUk l
;UUl m
}VV 
)VV 
)VV 
;VV 
}WW 
;WW 
musicPlayerYY 
.YY  
MediaFailedYY  +
+=YY, .
(YY/ 0
senderYY0 6
,YY6 7
eYY8 9
)YY9 :
=>YY; =
{ZZ 
isMusicLoaded[[ %
=[[& '
false[[( -
;[[- .
Debug]] 
.]] 
	WriteLine]] '
(]]' (
$"]]( *
$str]]* f
"]]f g
)]]g h
;]]h i
Debug^^ 
.^^ 
	WriteLine^^ '
(^^' (
$"^^( *
$str^^* e
{^^e f
tempMusicFilePath^^f w
}^^w x
"^^x y
)^^y z
;^^z {
if__ 
(__ 
e__ 
.__ 
ErrorException__ ,
!=__- /
null__0 4
)__4 5
{`` 
Debugaa !
.aa! "
	WriteLineaa" +
(aa+ ,
$"aa, .
$straa. A
{aaA B
eaaB C
.aaC D
ErrorExceptionaaD R
.aaR S
GetTypeaaS Z
(aaZ [
)aa[ \
.aa\ ]
FullNameaa] e
}aae f
"aaf g
)aag h
;aah i
Debugbb !
.bb! "
	WriteLinebb" +
(bb+ ,
$"bb, .
$strbb. D
{bbD E
ebbE F
.bbF G
ErrorExceptionbbG U
.bbU V
MessagebbV ]
}bb] ^
"bb^ _
)bb_ `
;bb` a
Debugcc !
.cc! "
	WriteLinecc" +
(cc+ ,
$"cc, .
$strcc. >
{cc> ?
ecc? @
.cc@ A
ErrorExceptionccA O
.ccO P

StackTraceccP Z
}ccZ [
"cc[ \
)cc\ ]
;cc] ^
ifdd 
(dd  
edd  !
.dd! "
ErrorExceptiondd" 0
.dd0 1
InnerExceptiondd1 ?
!=dd@ B
nullddC G
)ddG H
{ee 
Debugff  %
.ff% &
	WriteLineff& /
(ff/ 0
$"ff0 2
$strff2 F
{ffF G
effG H
.ffH I
ErrorExceptionffI W
.ffW X
InnerExceptionffX f
}fff g
"ffg h
)ffh i
;ffi j
}gg 
}hh 
elseii 
{jj 
Debugkk !
.kk! "
	WriteLinekk" +
(kk+ ,
$"kk, .
$strkk. U
"kkU V
)kkV W
;kkW X
}ll 
Debugmm 
.mm 
	WriteLinemm '
(mm' (
$"mm( *
$strmm* f
"mmf g
)mmg h
;mmh i
cleanupTempFilenn '
(nn' (
)nn( )
;nn) *
}oo 
;oo 
musicPlayerrr 
.rr  

MediaEndedrr  *
+=rr+ -
(rr. /
srr/ 0
,rr0 1
err2 3
)rr3 4
=>rr5 7
{ss 
musicPlayertt #
.tt# $
Positiontt$ ,
=tt- .
TimeSpantt/ 7
.tt7 8
Zerott8 <
;tt< =
musicPlayeruu #
.uu# $
Playuu$ (
(uu( )
)uu) *
;uu* +
Debugvv 
.vv 
	WriteLinevv '
(vv' (
$strvv( N
)vvN O
;vvO P
}ww 
;ww 
Debugzz 
.zz 
	WriteLinezz #
(zz# $
$"zz$ &
$strzz& c
{zzc d
fileUrizzd k
.zzk l
AbsoluteUrizzl w
}zzw x
"zzx y
)zzy z
;zzz {
musicPlayer{{ 
.{{  
Open{{  $
({{$ %
fileUri{{% ,
){{, -
;{{- .
}}} 
else~~ 
{ 
Debug
ÄÄ 
.
ÄÄ 
	WriteLine
ÄÄ #
(
ÄÄ# $
$"
ÄÄ$ &
$str
ÄÄ& k
{
ÄÄk l
resourcePath
ÄÄl x
}
ÄÄx y
$strÄÄy ò
"ÄÄò ô
)ÄÄô ö
;ÄÄö õ
isMusicLoaded
ÅÅ !
=
ÅÅ" #
false
ÅÅ$ )
;
ÅÅ) *
}
ÇÇ 
}
ÉÉ 
catch
ÑÑ 
(
ÑÑ 
	Exception
ÑÑ 
ex
ÑÑ 
)
ÑÑ  
{
ÖÖ 
Debug
ÜÜ 
.
ÜÜ 
	WriteLine
ÜÜ 
(
ÜÜ  
$"
ÜÜ  "
$str
ÜÜ" q
{
ÜÜq r
ex
ÜÜr t
}
ÜÜt u
"
ÜÜu v
)
ÜÜv w
;
ÜÜw x
isMusicLoaded
áá 
=
áá 
false
áá  %
;
áá% &
cleanupTempFile
àà 
(
àà  
)
àà  !
;
àà! "
}
ââ 
Application
ãã 
.
ãã 
Current
ãã 
.
ãã  
Exit
ãã  $
+=
ãã% '
OnApplicationExit
ãã( 9
;
ãã9 :
}
åå 	
private
èè 
static
èè 
void
èè 
cleanupTempFile
èè +
(
èè+ ,
)
èè, -
{
êê 	
if
ëë 
(
ëë 
tempMusicFilePath
ëë !
!=
ëë" $
null
ëë% )
&&
ëë* ,
File
ëë- 1
.
ëë1 2
Exists
ëë2 8
(
ëë8 9
tempMusicFilePath
ëë9 J
)
ëëJ K
)
ëëK L
{
íí 
try
ìì 
{
îî 
File
ïï 
.
ïï 
Delete
ïï 
(
ïï  
tempMusicFilePath
ïï  1
)
ïï1 2
;
ïï2 3
Debug
ññ 
.
ññ 
	WriteLine
ññ #
(
ññ# $
$"
ññ$ &
$str
ññ& R
{
ññR S
tempMusicFilePath
ññS d
}
ññd e
"
ññe f
)
ññf g
;
ññg h
tempMusicFilePath
óó %
=
óó& '
null
óó( ,
;
óó, -
}
òò 
catch
ôô 
(
ôô 
IOException
ôô "
ex
ôô# %
)
ôô% &
{
ôô' (
Debug
ôô) .
.
ôô. /
	WriteLine
ôô/ 8
(
ôô8 9
$"
ôô9 ;
$str
ôô; x
{
ôôx y 
tempMusicFilePathôôy ä
}ôôä ã
$strôôã ï
{ôôï ñ
exôôñ ò
.ôôò ô
Messageôôô †
}ôô† °
"ôô° ¢
)ôô¢ £
;ôô£ §
}ôô• ¶
catch
öö 
(
öö )
UnauthorizedAccessException
öö 2
ex
öö3 5
)
öö5 6
{
öö7 8
Debug
öö9 >
.
öö> ?
	WriteLine
öö? H
(
ööH I
$"
ööI K
$strööK è
{ööè ê!
tempMusicFilePathööê °
}öö° ¢
$ströö¢ ¨
{öö¨ ≠
exöö≠ Ø
.ööØ ∞
Messageöö∞ ∑
}öö∑ ∏
"öö∏ π
)ööπ ∫
;öö∫ ª
}ööº Ω
}
õõ 
}
úú 	
private
ùù 
static
ùù 
void
ùù 
OnApplicationExit
ùù -
(
ùù- .
object
ùù. 4
sender
ùù5 ;
,
ùù; <
ExitEventArgs
ùù= J
e
ùùK L
)
ùùL M
{
ûû 	
	stopMusic
üü 
(
üü 
)
üü 
;
üü 
musicPlayer
†† 
.
†† 
Close
†† 
(
†† 
)
†† 
;
††  
cleanupTempFile
°° 
(
°° 
)
°° 
;
°° 
Application
¢¢ 
.
¢¢ 
Current
¢¢ 
.
¢¢  
Exit
¢¢  $
-=
¢¢% '
OnApplicationExit
¢¢( 9
;
¢¢9 :
}
££ 	
private
§§ 
static
§§ 
void
§§  
loadInitialVolumes
§§ .
(
§§. /
)
§§/ 0
{
•• 	
try
¶¶ 
{
ßß 
double
®®  
initialMusicVolume
®® )
=
®®* +
Settings
®®, 4
.
®®4 5
Default
®®5 <
.
®®< = 
MusicVolumeSetting
®®= O
;
®®O P
double
©© 
initialSfxVolume
©© '
=
©©( )
Settings
©©* 2
.
©©2 3
Default
©©3 :
.
©©: ;'
SoundEffectsVolumeSetting
©©; T
;
©©T U$
setMusicVolumeInternal
™™ &
(
™™& ' 
initialMusicVolume
™™' 9
/
™™: ;
$num
™™< A
)
™™A B
;
™™B C+
setSoundEffectsVolumeInternal
´´ -
(
´´- .
initialSfxVolume
´´. >
/
´´? @
$num
´´A F
)
´´F G
;
´´G H
Debug
¨¨ 
.
¨¨ 
	WriteLine
¨¨ 
(
¨¨  
$"
¨¨  "
$str
¨¨" Z
{
¨¨Z [
musicPlayer
¨¨[ f
.
¨¨f g
Volume
¨¨g m
}
¨¨m n
$str
¨¨n t
{
¨¨t u
	sfxPlayer
¨¨u ~
.
¨¨~ 
Volume¨¨ Ö
}¨¨Ö Ü
"¨¨Ü á
)¨¨á à
;¨¨à â
}
≠≠ 
catch
ÆÆ 
(
ÆÆ 
	Exception
ÆÆ 
ex
ÆÆ 
)
ÆÆ  
{
ØØ 
Debug
∞∞ 
.
∞∞ 
	WriteLine
∞∞ 
(
∞∞  
$"
∞∞  "
$str
∞∞" e
{
∞∞e f
ex
∞∞f h
.
∞∞h i
Message
∞∞i p
}
∞∞p q
"
∞∞q r
)
∞∞r s
;
∞∞s t$
setMusicVolumeInternal
±± &
(
±±& '
$num
±±' *
)
±±* +
;
±±+ ,+
setSoundEffectsVolumeInternal
≤≤ -
(
≤≤- .
$num
≤≤. 1
)
≤≤1 2
;
≤≤2 3
}
≥≥ 
}
¥¥ 	
private
µµ 
static
µµ 
void
µµ $
setMusicVolumeInternal
µµ 2
(
µµ2 3
double
µµ3 9
volume
µµ: @
)
µµ@ A
{
µµB C
if
µµD F
(
µµG H
volume
µµH N
<
µµO P
$num
µµQ R
)
µµR S
volume
µµT Z
=
µµ[ \
$num
µµ] ^
;
µµ^ _
if
µµ` b
(
µµc d
volume
µµd j
>
µµk l
$num
µµm n
)
µµn o
volume
µµp v
=
µµw x
$num
µµy z
;
µµz {
musicPlayerµµ| á
.µµá à
Volumeµµà é
=µµè ê
volumeµµë ó
;µµó ò
}µµô ö
private
∂∂ 
static
∂∂ 
void
∂∂ +
setSoundEffectsVolumeInternal
∂∂ 9
(
∂∂9 :
double
∂∂: @
volume
∂∂A G
)
∂∂G H
{
∂∂I J
if
∂∂K M
(
∂∂N O
volume
∂∂O U
<
∂∂V W
$num
∂∂X Y
)
∂∂Y Z
volume
∂∂[ a
=
∂∂b c
$num
∂∂d e
;
∂∂e f
if
∂∂g i
(
∂∂j k
volume
∂∂k q
>
∂∂r s
$num
∂∂t u
)
∂∂u v
volume
∂∂w }
=
∂∂~ 
$num∂∂Ä Å
;∂∂Å Ç
	sfxPlayer∂∂É å
.∂∂å ç
Volume∂∂ç ì
=∂∂î ï
volume∂∂ñ ú
;∂∂ú ù
}∂∂û ü
public
∑∑ 
static
∑∑ 
void
∑∑ 
setMusicVolume
∑∑ )
(
∑∑) *
double
∑∑* 0
volume
∑∑1 7
)
∑∑7 8
{
∑∑9 :$
setMusicVolumeInternal
∑∑; Q
(
∑∑Q R
volume
∑∑R X
)
∑∑X Y
;
∑∑Y Z
Debug
∑∑[ `
.
∑∑` a
	WriteLine
∑∑a j
(
∑∑j k
$"
∑∑k m
$str∑∑m ê
{∑∑ê ë
musicPlayer∑∑ë ú
.∑∑ú ù
Volume∑∑ù £
}∑∑£ §
"∑∑§ •
)∑∑• ¶
;∑∑¶ ß
}∑∑® ©
public
∏∏ 
static
∏∏ 
void
∏∏ #
setSoundEffectsVolume
∏∏ 0
(
∏∏0 1
double
∏∏1 7
volume
∏∏8 >
)
∏∏> ?
{
∏∏@ A+
setSoundEffectsVolumeInternal
∏∏B _
(
∏∏_ `
volume
∏∏` f
)
∏∏f g
;
∏∏g h
Debug
∏∏i n
.
∏∏n o
	WriteLine
∏∏o x
(
∏∏x y
$"
∏∏y {
$str∏∏{ ú
{∏∏ú ù
	sfxPlayer∏∏ù ¶
.∏∏¶ ß
Volume∏∏ß ≠
}∏∏≠ Æ
"∏∏Æ Ø
)∏∏Ø ∞
;∏∏∞ ±
}∏∏≤ ≥
public
ππ 
static
ππ 
void
ππ 
	playMusic
ππ $
(
ππ$ %
)
ππ% &
{
ππ' (
if
ππ) +
(
ππ, -
isMusicLoaded
ππ- :
&&
ππ; =
tempMusicFilePath
ππ> O
!=
ππP R
null
ππS W
&&
ππX Z
File
ππ[ _
.
ππ_ `
Exists
ππ` f
(
ππf g
tempMusicFilePath
ππg x
)
ππx y
)
ππy z
{
ππ{ |
tryππ} Ä
{ππÅ Ç
musicPlayerππÉ é
.ππé è
Playππè ì
(ππì î
)ππî ï
;ππï ñ
Debugππó ú
.ππú ù
	WriteLineππù ¶
(ππ¶ ß
$strππß ”
)ππ” ‘
;ππ‘ ’
}ππ÷ ◊
catchππÿ ›
(ππﬁ ﬂ
	Exceptionππﬂ Ë
exππÈ Î
)ππÎ Ï
{ππÌ Ó
DebugππÔ Ù
.ππÙ ı
	WriteLineππı ˛
(ππ˛ ˇ
$"ππˇ Å
$strππÅ ™
{ππ™ ´
exππ´ ≠
.ππ≠ Æ
MessageππÆ µ
}ππµ ∂
"ππ∂ ∑
)ππ∑ ∏
;ππ∏ π
}ππ∫ ª
}ππº Ω
elseππæ ¬
{ππ√ ƒ
Debugππ≈  
.ππ  À
	WriteLineππÀ ‘
(ππ‘ ’
$"ππ’ ◊
$strππ◊ ç
"ππç é
)ππé è
;ππè ê
}ππë í
}ππì î
public
∫∫ 
static
∫∫ 
void
∫∫ 
	stopMusic
∫∫ $
(
∫∫$ %
)
∫∫% &
{
∫∫' (
try
∫∫) ,
{
∫∫- .
if
∫∫/ 1
(
∫∫2 3
musicPlayer
∫∫3 >
.
∫∫> ?
CanPause
∫∫? G
)
∫∫G H
{
∫∫I J
musicPlayer
∫∫K V
.
∫∫V W
Stop
∫∫W [
(
∫∫[ \
)
∫∫\ ]
;
∫∫] ^
Debug
∫∫_ d
.
∫∫d e
	WriteLine
∫∫e n
(
∫∫n o
$str∫∫o í
)∫∫í ì
;∫∫ì î
}∫∫ï ñ
else∫∫ó õ
{∫∫ú ù
Debug∫∫û £
.∫∫£ §
	WriteLine∫∫§ ≠
(∫∫≠ Æ
$"∫∫Æ ∞
$str∫∫∞ ·
"∫∫· ‚
)∫∫‚ „
;∫∫„ ‰
}∫∫Â Ê
}∫∫Á Ë
catch∫∫È Ó
(∫∫Ô 
	Exception∫∫ ˘
ex∫∫˙ ¸
)∫∫¸ ˝
{∫∫˛ ˇ
Debug∫∫Ä Ö
.∫∫Ö Ü
	WriteLine∫∫Ü è
(∫∫è ê
$"∫∫ê í
$str∫∫í ¥
{∫∫¥ µ
ex∫∫µ ∑
.∫∫∑ ∏
Message∫∫∏ ø
}∫∫ø ¿
"∫∫¿ ¡
)∫∫¡ ¬
;∫∫¬ √
}∫∫ƒ ≈
}∫∫∆ «
public
ææ 
static
ææ 
void
ææ 
playSoundEffect
ææ *
(
ææ* +
string
ææ+ 1
soundFileName
ææ2 ?
)
ææ? @
{
øø 	
if
¿¿ 
(
¿¿ 
string
¿¿ 
.
¿¿  
IsNullOrWhiteSpace
¿¿ )
(
¿¿) *
soundFileName
¿¿* 7
)
¿¿7 8
)
¿¿8 9
{
¿¿: ;
Debug
¿¿< A
.
¿¿A B
	WriteLine
¿¿B K
(
¿¿K L
$str¿¿L Å
)¿¿Å Ç
;¿¿Ç É
return¿¿Ñ ä
;¿¿ä ã
}¿¿å ç
try
¡¡ 
{
¬¬ 
Uri
√√ 
sfxUri
√√ 
=
√√ 
new
√√  
Uri
√√! $
(
√√$ %
$"
√√% '
$str
√√' h
{
√√h i
soundFileName
√√i v
}
√√v w
"
√√w x
,
√√x y
UriKind√√z Å
.√√Å Ç
Absolute√√Ç ä
)√√ä ã
;√√ã å
	sfxPlayer
ƒƒ 
.
ƒƒ 
Open
ƒƒ 
(
ƒƒ 
sfxUri
ƒƒ %
)
ƒƒ% &
;
ƒƒ& '
	sfxPlayer
≈≈ 
.
≈≈ 
Play
≈≈ 
(
≈≈ 
)
≈≈  
;
≈≈  !
Debug
∆∆ 
.
∆∆ 
	WriteLine
∆∆ 
(
∆∆  
$"
∆∆  "
$str
∆∆" =
{
∆∆= >
soundFileName
∆∆> K
}
∆∆K L
$str
∆∆L N
"
∆∆N O
)
∆∆O P
;
∆∆P Q
}
«« 
catch
»» 
(
»» 
	Exception
»» 
ex
»» 
)
»»  
{
»»! "
Debug
»»# (
.
»»( )
	WriteLine
»») 2
(
»»2 3
$"
»»3 5
$str
»»5 V
{
»»V W
soundFileName
»»W d
}
»»d e
$str
»»e i
{
»»i j
ex
»»j l
.
»»l m
Message
»»m t
}
»»t u
"
»»u v
)
»»v w
;
»»w x
}
»»y z
}
…… 	
}
   
}ÀÀ ¯k
ñC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Services\SocialServiceClientManager.cs
	namespace 	
MindWeaveClient
 
. 
Services "
{ 
public 

sealed 
class &
SocialServiceClientManager 2
{		 
private

 
static

 
readonly

 
Lazy

  $
<

$ %&
SocialServiceClientManager

% ?
>

? @
lazy

A E
=

F G
new 
Lazy 
< &
SocialServiceClientManager /
>/ 0
(0 1
(1 2
)2 3
=>4 6
new7 :&
SocialServiceClientManager; U
(U V
)V W
)W X
;X Y
public 
static &
SocialServiceClientManager 0
instance1 9
{ 	
get 
{ 
return 
lazy 
. 
Value #
;# $
}% &
} 	
public 
SocialManagerClient "
proxy# (
{) *
get+ .
;. /
private0 7
set8 ;
;; <
}= >
public !
SocialCallbackHandler $
callbackHandler% 4
{5 6
get7 :
;: ;
private< C
setD G
;G H
}I J
private 
InstanceContext 
site  $
;$ %
private 
string 
connectedUsername (
;( )
private &
SocialServiceClientManager *
(* +
)+ ,
{ 	
} 	
public 
bool 
Connect 
( 
string "
username# +
)+ ,
{ 	
if 
( 
string 
. 
IsNullOrWhiteSpace )
() *
username* 2
)2 3
)3 4
{ 
Console 
. 
	WriteLine !
(! "
$str" W
)W X
;X Y
return   
false   
;   
}!! 
try## 
{$$ 
if%% 
(%% 
proxy%% 
!=%% 
null%% !
&&%%" $
proxy%%% *
.%%* +
State%%+ 0
==%%1 3
CommunicationState%%4 F
.%%F G
Opened%%G M
&&%%N P
connectedUsername%%Q b
==%%c e
username%%f n
)%%n o
{&& 
Console'' 
.'' 
	WriteLine'' %
(''% &
$"''& (
$str''( R
{''R S
username''S [
}''[ \
$str''\ ]
"''] ^
)''^ _
;''_ `
return(( 
true(( 
;((  
})) 
if++ 
(++ 
proxy++ 
!=++ 
null++ !
)++! "
{,, 
Console-- 
.-- 
	WriteLine-- %
(--% &
$".. 
$str.. =
{..= >
proxy..> C
...C D
State..D I
}..I J
$str..J 
"	.. Ä
)
..Ä Å
;
..Å Ç

Disconnect// 
(// 
)//  
;//  !
}00 
Console33 
.33 
	WriteLine33 !
(33! "
$"33" $
$str33$ R
{33R S
username33S [
}33[ \
$str33\ _
"33_ `
)33` a
;33a b
callbackHandler44 
=44  !
new44" %!
SocialCallbackHandler44& ;
(44; <
)44< =
;44= >
site55 
=55 
new55 
InstanceContext55 *
(55* +
callbackHandler55+ :
)55: ;
;55; <
proxy66 
=66 
new66 
SocialManagerClient66 /
(66/ 0
site660 4
,664 5
$str666 T
)66T U
;66U V
proxy88 
.88 
Open88 
(88 
)88 
;88 
Console99 
.99 
	WriteLine99 !
(99! "
$str99" F
)99F G
;99G H
connectedUsername;; !
=;;" #
username;;$ ,
;;;, -
Task<< 
.<< 
Run<< 
(<< 
async<< 
(<<  
)<<  !
=><<" $
{== 
try>> 
{?? 
await@@ 
proxy@@ #
.@@# $
connectAsync@@$ 0
(@@0 1
username@@1 9
)@@9 :
;@@: ;
ConsoleAA 
.AA  
	WriteLineAA  )
(AA) *
$"AA* ,
$strAA, I
{AAI J
usernameAAJ R
}AAR S
$strAAS j
"AAj k
)AAk l
;AAl m
}BB 
catchCC 
(CC 
	ExceptionCC $
exCC% '
)CC' (
{DD 
ConsoleEE 
.EE  
	WriteLineEE  )
(EE) *
$"EE* ,
$strEE, K
{EEK L
usernameEEL T
}EET U
$strEEU W
{EEW X
exEEX Z
.EEZ [
MessageEE[ b
}EEb c
"EEc d
)EEd e
;EEe f

DisconnectFF "
(FF" #
)FF# $
;FF$ %
}GG 
}HH 
)HH 
;HH 
returnKK 
trueKK 
;KK 
}LL 
catchMM 
(MM 
	ExceptionMM 
exMM 
)MM  
{NN 
ConsoleOO 
.OO 
	WriteLineOO !
(OO! "
$"OO" $
$strOO$ H
{OOH I
usernameOOI Q
}OOQ R
$strOOR T
{OOT U
exOOU W
.OOW X
MessageOOX _
}OO_ `
"OO` a
)OOa b
;OOb c

DisconnectPP 
(PP 
)PP 
;PP 
returnQQ 
falseQQ 
;QQ 
}RR 
}SS 	
publicUU 
voidUU 

DisconnectUU 
(UU 
)UU  
{VV 	
stringWW 
userToDisconnectWW #
=WW$ %
connectedUsernameWW& 7
;WW7 8
ConsoleXX 
.XX 
	WriteLineXX 
(XX 
$"YY 
$strYY >
{YY> ?
userToDisconnectYY? O
}YYO P
$strYYP _
{YY_ `
proxyYY` e
?YYe f
.YYf g
StateYYg l
}YYl m
"YYm n
)YYn o
;YYo p
if[[ 
([[ 
proxy[[ 
!=[[ 
null[[ 
&&[[  
proxy[[! &
.[[& '
State[[' ,
==[[- /
CommunicationState[[0 B
.[[B C
Opened[[C I
&&[[J L
![[M N
string[[N T
.[[T U
IsNullOrEmpty[[U b
([[b c
userToDisconnect[[c s
)[[s t
)[[t u
{\\ 
Task]] 
.]] 
Run]] 
(]] 
async]] 
(]]  
)]]  !
=>]]" $
{^^ 
try__ 
{`` 
awaitaa 
proxyaa #
.aa# $
disconnectAsyncaa$ 3
(aa3 4
userToDisconnectaa4 D
)aaD E
;aaE F
Consolebb 
.bb  
	WriteLinebb  )
(bb) *
$"bb* ,
$strbb, L
{bbL M
userToDisconnectbbM ]
}bb] ^
$strbb^ u
"bbu v
)bbv w
;bbw x
}cc 
catchdd 
(dd 
	Exceptiondd $
exdd% '
)dd' (
{ee 
Consoleff 
.ff  
	WriteLineff  )
(ff) *
$"ff* ,
$strff, N
{ffN O
userToDisconnectffO _
}ff_ `
$strff` b
{ffb c
exffc e
.ffe f
Messagefff m
}ffm n
"ffn o
)ffo p
;ffp q
}gg 
}hh 
)hh 
;hh 
}ii 
elsejj 
ifjj 
(jj 
!jj 
stringjj 
.jj 
IsNullOrEmptyjj *
(jj* +
userToDisconnectjj+ ;
)jj; <
)jj< =
{kk 
Consolell 
.ll 
	WriteLinell !
(ll! "
$"ll" $
$strll$ R
{llR S
proxyllS X
?llX Y
.llY Z
StatellZ _
}ll_ `
$strll` b
"llb c
)llc d
;lld e
}mm 
tryoo 
{pp 
ifqq 
(qq 
proxyqq 
!=qq 
nullqq !
)qq! "
{rr 
ifss 
(ss 
proxyss 
.ss 
Statess #
==ss$ &
CommunicationStatess' 9
.ss9 :
Openedss: @
||ssA C
proxyssD I
.ssI J
StatessJ O
==ssP R
CommunicationStatessS e
.sse f
Openingssf m
)ssm n
{tt 
proxyuu 
.uu 
Closeuu #
(uu# $
)uu$ %
;uu% &
Consolevv 
.vv  
	WriteLinevv  )
(vv) *
$strvv* N
)vvN O
;vvO P
}ww 
elsexx 
ifxx 
(xx 
proxyxx "
.xx" #
Statexx# (
!=xx) +
CommunicationStatexx, >
.xx> ?
Closedxx? E
)xxE F
{yy 
proxyzz 
.zz 
Abortzz #
(zz# $
)zz$ %
;zz% &
Console{{ 
.{{  
	WriteLine{{  )
({{) *
$"{{* ,
$str{{, Z
{{{Z [
proxy{{[ `
.{{` a
State{{a f
}{{f g
$str{{g h
"{{h i
){{i j
;{{j k
}|| 
}}} 
}~~ 
catch 
( 
	Exception 
ex 
)  
{
ÄÄ 
Console
ÅÅ 
.
ÅÅ 
	WriteLine
ÅÅ !
(
ÅÅ! "
$"
ÅÅ" $
$str
ÅÅ$ N
{
ÅÅN O
ex
ÅÅO Q
.
ÅÅQ R
Message
ÅÅR Y
}
ÅÅY Z
$str
ÅÅZ e
"
ÅÅe f
)
ÅÅf g
;
ÅÅg h
proxy
ÇÇ 
?
ÇÇ 
.
ÇÇ 
Abort
ÇÇ 
(
ÇÇ 
)
ÇÇ 
;
ÇÇ 
}
ÉÉ 
finally
ÑÑ 
{
ÖÖ 
proxy
ÜÜ 
=
ÜÜ 
null
ÜÜ 
;
ÜÜ 
site
áá 
=
áá 
null
áá 
;
áá 
callbackHandler
àà 
=
àà  !
null
àà" &
;
àà& '
connectedUsername
ââ !
=
ââ" #
null
ââ$ (
;
ââ( )
Console
ää 
.
ää 
	WriteLine
ää !
(
ää! "
$str
ää" N
)
ääN O
;
ääO P
}
ãã 
}
åå 	
public
éé 
bool
éé 
EnsureConnected
éé #
(
éé# $
string
éé$ *
username
éé+ 3
)
éé3 4
{
èè 	
if
êê 
(
êê 
string
êê 
.
êê  
IsNullOrWhiteSpace
êê )
(
êê) *
username
êê* 2
)
êê2 3
)
êê3 4
return
êê5 ;
false
êê< A
;
êêA B
if
íí 
(
íí 
proxy
íí 
==
íí 
null
íí 
||
íí  
proxy
íí! &
.
íí& '
State
íí' ,
==
íí- / 
CommunicationState
íí0 B
.
ííB C
Closed
ííC I
||
ííJ L
proxy
ìì 
.
ìì 
State
ìì 
==
ìì  
CommunicationState
ìì 1
.
ìì1 2
Faulted
ìì2 9
||
ìì: <
connectedUsername
ìì= N
!=
ììO Q
username
ììR Z
)
ììZ [
{
îî 
Console
ïï 
.
ïï 
	WriteLine
ïï !
(
ïï! "
$"
ññ 
$str
ññ E
{
ññE F
username
ññF N
}
ññN O
$str
ññO `
{
ññ` a
proxy
ñña f
?
ññf g
.
ññg h
State
ññh m
}
ññm n
$str
ññn ~
{
ññ~  
connectedUsernameññ ê
}ññê ë
"ññë í
)ññí ì
;ññì î
return
óó 
Connect
óó 
(
óó 
username
óó '
)
óó' (
;
óó( )
}
òò 
if
öö 
(
öö 
proxy
öö 
.
öö 
State
öö 
==
öö  
CommunicationState
öö 1
.
öö1 2
Opening
öö2 9
||
öö: <
proxy
öö= B
.
ööB C
State
ööC H
==
ööI K 
CommunicationState
ööL ^
.
öö^ _
Created
öö_ f
)
ööf g
{
õõ 
Console
úú 
.
úú 
	WriteLine
úú !
(
úú! "
$"
úú" $
$str
úú$ D
{
úúD E
proxy
úúE J
.
úúJ K
State
úúK P
}
úúP Q
$str
úúQ W
{
úúW X
username
úúX `
}
úú` a
$str
úúa m
"
úúm n
)
úún o
;
úúo p
return
ùù 
false
ùù 
;
ùù 
}
ûû 
return
†† 
proxy
†† 
.
†† 
State
†† 
==
†† ! 
CommunicationState
††" 4
.
††4 5
Opened
††5 ;
&&
††< >
connectedUsername
††? P
==
††Q S
username
††T \
;
††\ ]
}
°° 	
}
¢¢ 
}££ €%
ëC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Services\SocialCallbackHandler.cs
	namespace 	
MindWeaveClient
 
. 
Services "
{ 
public 

class !
SocialCallbackHandler &
:' ("
ISocialManagerCallback) ?
{		 
public

 
event

 
Action

 
<

 
string

 "
>

" #!
FriendRequestReceived

$ 9
;

9 :
public 
event 
Action 
< 
string "
," #
bool$ (
>( )"
FriendResponseReceived* @
;@ A
public 
event 
Action 
< 
string "
," #
bool$ (
>( )
FriendStatusChanged* =
;= >
public 
event 
Action 
< 
string "
," #
string$ *
>* +
LobbyInviteReceived, ?
;? @
public 
void 
notifyFriendRequest '
(' (
string( .
fromUsername/ ;
); <
{ 	
Debug 
. 
	WriteLine 
( 
$" 
$str E
{E F
fromUsernameF R
}R S
"S T
)T U
;U V
Application 
. 
Current 
.  

Dispatcher  *
.* +
Invoke+ 1
(1 2
(2 3
)3 4
=>5 7
{ !
FriendRequestReceived %
?% &
.& '
Invoke' -
(- .
fromUsername. :
): ;
;; <
} 
) 
; 
} 	
public 
void  
notifyFriendResponse (
(( )
string) /
fromUsername0 <
,< =
bool> B
acceptedC K
)K L
{ 	
Debug 
. 
	WriteLine 
( 
$" 
$str =
{= >
fromUsername> J
}J K
$strK W
{W X
acceptedX `
}` a
"a b
)b c
;c d
Application 
. 
Current 
.  

Dispatcher  *
.* +
Invoke+ 1
(1 2
(2 3
)3 4
=>5 7
{ "
FriendResponseReceived &
?& '
.' (
Invoke( .
(. /
fromUsername/ ;
,; <
accepted= E
)E F
;F G
} 
) 
; 
} 	
public!! 
void!! %
notifyFriendStatusChanged!! -
(!!- .
string!!. 4
friendUsername!!5 C
,!!C D
bool!!E I
isOnline!!J R
)!!R S
{"" 	
Debug## 
.## 
	WriteLine## 
(## 
$"## 
$str## B
{##B C
friendUsername##C Q
}##Q R
$str##R \
{##\ ]
isOnline##] e
}##e f
"##f g
)##g h
;##h i
Application$$ 
.$$ 
Current$$ 
.$$  

Dispatcher$$  *
.$$* +
Invoke$$+ 1
($$1 2
($$2 3
)$$3 4
=>$$5 7
{%% 
FriendStatusChanged&& #
?&&# $
.&&$ %
Invoke&&% +
(&&+ ,
friendUsername&&, :
,&&: ;
isOnline&&< D
)&&D E
;&&E F
}'' 
)'' 
;'' 
}(( 	
public** 
void** 
notifyLobbyInvite** %
(**% &
string**& ,
fromUsername**- 9
,**9 :
string**; A
lobbyId**B I
)**I J
{++ 	
Debug,, 
.,, 
	WriteLine,, 
(,, 
$",, 
$str,, C
{,,C D
fromUsername,,D P
},,P Q
$str,,Q \
{,,\ ]
lobbyId,,] d
},,d e
",,e f
),,f g
;,,g h
Application-- 
.-- 
Current-- 
.--  

Dispatcher--  *
.--* +
Invoke--+ 1
(--1 2
(--2 3
)--3 4
=>--5 7
{.. 
LobbyInviteReceived// #
?//# $
.//$ %
Invoke//% +
(//+ ,
fromUsername//, 8
,//8 9
lobbyId//: A
)//A B
;//B C
}00 
)00 
;00 
}11 	
}33 
}44 ‹&
ñC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Services\MatchmakingCallbackHandler.cs
	namespace 	
MindWeaveClient
 
. 
Services "
{		 
public

 

class

 &
MatchmakingCallbackHandler

 +
:

, -'
IMatchmakingManagerCallback

. I
{ 
public 
event 
Action 
< 
LobbyStateDto )
>) *
LobbyStateUpdated+ <
;< =
public 
event 
Action 
< 
string "
," #
List$ (
<( )
string) /
>/ 0
>0 1

MatchFound2 <
;< =
public 
event 
Action 
< 
string "
>" #
LobbyCreationFailed$ 7
;7 8
public 
event 
Action 
< 
string "
>" #
KickedFromLobby$ 3
;3 4
public 
void 
updateLobbyState $
($ %
LobbyStateDto% 2
lobbyStateDto3 @
)@ A
{ 	
Debug 
. 
	WriteLine 
( 
$" 
$str F
{F G
lobbyStateDtoG T
?T U
.U V
lobbyIdV ]
}] ^
"^ _
)_ `
;` a
Application 
. 
Current 
.  

Dispatcher  *
.* +
Invoke+ 1
(1 2
(2 3
)3 4
=>5 7
{8 9
LobbyStateUpdated: K
?K L
.L M
InvokeM S
(S T
lobbyStateDtoT a
)a b
;b c
}d e
)e f
;f g
} 	
public 
void 

matchFound 
( 
string %
matchId& -
,- .
string/ 5
[5 6
]6 7
players8 ?
)? @
{ 	
List 
< 
string 
> 

playerList #
=$ %
players& -
?- .
.. /
ToList/ 5
(5 6
)6 7
??8 :
new; >
List? C
<C D
stringD J
>J K
(K L
)L M
;M N
Debug 
. 
	WriteLine 
( 
$" 
$str 9
{9 :
matchId: A
}A B
$strB M
{M N
stringN T
.T U
JoinU Y
(Y Z
$strZ ^
,^ _

playerList` j
)j k
}k l
"l m
)m n
;n o
Application 
. 
Current 
.  

Dispatcher  *
.* +
Invoke+ 1
(1 2
(2 3
)3 4
=>5 7
{8 9

MatchFound: D
?D E
.E F
InvokeF L
(L M
matchIdM T
,T U

playerListV `
)` a
;a b
}c d
)d e
;e f
} 	
public"" 
void"" 
lobbyCreationFailed"" '
(""' (
string""( .
reason""/ 5
)""5 6
{## 	
Debug$$ 
.$$ 
	WriteLine$$ 
($$ 
$"$$ 
$str$$ G
{$$G H
reason$$H N
}$$N O
"$$O P
)$$P Q
;$$Q R
Application%% 
.%% 
Current%% 
.%%  

Dispatcher%%  *
.%%* +
Invoke%%+ 1
(%%1 2
(%%2 3
)%%3 4
=>%%5 7
{%%8 9
LobbyCreationFailed%%: M
?%%M N
.%%N O
Invoke%%O U
(%%U V
reason%%V \
)%%\ ]
;%%] ^
}%%_ `
)%%` a
;%%a b
}&& 	
public(( 
void(( 
kickedFromLobby(( #
(((# $
string(($ *
reason((+ 1
)((1 2
{)) 	
Debug** 
.** 
	WriteLine** 
(** 
$"** 
$str** C
{**C D
reason**D J
}**J K
"**K L
)**L M
;**M N
Application++ 
.++ 
Current++ 
.++  

Dispatcher++  *
.++* +
Invoke+++ 1
(++1 2
(++2 3
)++3 4
=>++5 7
{++8 9
KickedFromLobby++: I
?++I J
.++J K
Invoke++K Q
(++Q R
reason++R X
)++X Y
;++Y Z
}++[ \
)++\ ]
;++] ^
},, 	
}-- 
}.. ˜
äC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Services\SessionService.cs
	namespace 	
MindWeaveClient
 
. 
Services "
{ 
public 

static 
class 
SessionService &
{ 
public 
static 
string 
Username %
{& '
get( +
;+ ,
private- 4
set5 8
;8 9
}: ;
public 
static 
string 

AvatarPath '
{( )
get* -
;- .
private/ 6
set7 :
;: ;
}< =
public 
static 
bool 
IsGuest "
{# $
get% (
;( )
private* 1
set2 5
;5 6
}7 8
public		 
static		 
void		 

SetSession		 %
(		% &
string		& ,
username		- 5
,		5 6
string		7 =

avatarPath		> H
,		H I
bool		J N
isGuest		O V
=		W X
false		Y ^
)		^ _
{

 	
SessionService 
. 
Username #
=$ %
username& .
;. /
SessionService 
. 

AvatarPath %
=& '

avatarPath( 2
??3 5
$str6 c
;c d
SessionService 
. 
IsGuest "
=# $
isGuest% ,
;, -
} 	
public 
static 
void 
UpdateAvatarPath +
(+ ,
string, 2
newAvatarPath3 @
)@ A
{ 	
if 
( 
! 
IsGuest 
) 
{ 

AvatarPath 
= 
newAvatarPath *
;* +
} 
} 	
public 
static 
void 
ClearSession '
(' (
)( )
{ 	
Username 
= 
null 
; 

AvatarPath 
= 
null 
; 
IsGuest 
= 
false 
; 
} 	
} 
} ∆O
îC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Services\ChatServiceClientManager.cs
	namespace 	
MindWeaveClient
 
. 
Services "
{ 
public 

sealed 
class $
ChatServiceClientManager 0
{ 
private		 
static		 
readonly		 
Lazy		  $
<		$ %$
ChatServiceClientManager		% =
>		= >
lazy		? C
=		D E
new

 
Lazy

 
<

 $
ChatServiceClientManager

 -
>

- .
(

. /
(

/ 0
)

0 1
=>

2 4
new

5 8$
ChatServiceClientManager

9 Q
(

Q R
)

R S
)

S T
;

T U
public 
static $
ChatServiceClientManager .
instance/ 7
{8 9
get: =
{> ?
return@ F
lazyG K
.K L
ValueL Q
;Q R
}S T
}U V
public 
ChatManagerClient  
proxy! &
{' (
get) ,
;, -
private. 5
set6 9
;9 :
}; <
public 
ChatCallbackHandler "
callbackHandler# 2
{3 4
get5 8
;8 9
private: A
setB E
;E F
}G H
private 
InstanceContext 
site  $
;$ %
private 
string 
connectedLobbyId '
;' (
private 
string 
connectedUsername (
;( )
private $
ChatServiceClientManager (
(( )
)) *
{+ ,
}- .
public 
bool 
Connect 
( 
string "
username# +
,+ ,
string- 3
lobbyId4 ;
); <
{ 	
if 
( 
string 
. 
IsNullOrWhiteSpace )
() *
username* 2
)2 3
||4 6
string7 =
.= >
IsNullOrWhiteSpace> P
(P Q
lobbyIdQ X
)X Y
)Y Z
{ 
Console 
. 
	WriteLine !
(! "
$str" X
)X Y
;Y Z
return 
false 
; 
} 
if 
( 
proxy 
!= 
null 
&&  
proxy! &
.& '
State' ,
==- /
CommunicationState0 B
.B C
OpenedC I
&&J L
connectedUsernameM ^
==_ a
usernameb j
&&k m
connectedLobbyIdn ~
==	 Å
lobbyId
Ç â
)
â ä
{   
Console!! 
.!! 
	WriteLine!! !
(!!! "
$"!!" $
$str!!$ O
{!!O P
username!!P X
}!!X Y
$str!!Y b
{!!b c
lobbyId!!c j
}!!j k
$str!!k l
"!!l m
)!!m n
;!!n o
return"" 
true"" 
;"" 
}## 
if%% 
(%% 
proxy%% 
!=%% 
null%% 
)%% 
{&& 
Console'' 
.'' 
	WriteLine'' !
(''! "
$"''" $
$str''$ b
{''b c
proxy''c h
.''h i
State''i n
}''n o
$str''o w
{''w x
connectedUsername	''x â
}
''â ä
$str
''ä ì
{
''ì î
connectedLobbyId
''î §
}
''§ •
$str
''• ª
"
''ª º
)
''º Ω
;
''Ω æ

Disconnect(( 
((( 
)(( 
;(( 
})) 
try,, 
{-- 
Console.. 
... 
	WriteLine.. !
(..! "
$".." $
$str..$ S
{..S T
username..T \
}..\ ]
$str..] f
{..f g
lobbyId..g n
}..n o
"..o p
)..p q
;..q r
callbackHandler// 
=//  !
new//" %
ChatCallbackHandler//& 9
(//9 :
)//: ;
;//; <
site00 
=00 
new00 
InstanceContext00 *
(00* +
callbackHandler00+ :
)00: ;
;00; <
proxy11 
=11 
new11 
ChatManagerClient11 -
(11- .
site11. 2
,112 3
$str114 P
)11P Q
;11Q R
proxy33 
.33 
Open33 
(33 
)33 
;33 
Console44 
.44 
	WriteLine44 !
(44! "
$str44" F
)44F G
;44G H
connectedUsername66 !
=66" #
username66$ ,
;66, -
connectedLobbyId77  
=77! "
lobbyId77# *
;77* +
proxy99 
.99 
joinLobbyChatAsync99 (
(99( )
username99) 1
,991 2
lobbyId993 :
)99: ;
;99; <
Console:: 
.:: 
	WriteLine:: !
(::! "
$"::" $
$str::$ G
{::G H
username::H P
}::P Q
$str::Q U
{::U V
lobbyId::V ]
}::] ^
$str::^ h
"::h i
)::i j
;::j k
return<< 
true<< 
;<< 
}== 
catch>> 
(>> 
	Exception>> 
ex>> 
)>>  
{?? 
Console@@ 
.@@ 
	WriteLine@@ !
(@@! "
$"@@" $
$str@@$ E
{@@E F
ex@@F H
.@@H I
Message@@I P
}@@P Q
"@@Q R
)@@R S
;@@S T

DisconnectAA 
(AA 
)AA 
;AA 
returnBB 
falseBB 
;BB 
}CC 
}DD 	
publicFF 
voidFF 

DisconnectFF 
(FF 
)FF  
{GG 	
stringHH 
userToDisconnectHH #
=HH$ %
connectedUsernameHH& 7
;HH7 8
stringII 
lobbyToDisconnectII $
=II% &
connectedLobbyIdII' 7
;II7 8
ConsoleJJ 
.JJ 
	WriteLineJJ 
(JJ 
$"JJ  
$strJJ  F
{JJF G
userToDisconnectJJG W
}JJW X
$strJJX a
{JJa b
lobbyToDisconnectJJb s
}JJs t
$str	JJt É
{
JJÉ Ñ
proxy
JJÑ â
?
JJâ ä
.
JJä ã
State
JJã ê
}
JJê ë
"
JJë í
)
JJí ì
;
JJì î
ifMM 
(MM 
proxyMM 
!=MM 
nullMM 
&&MM  
proxyMM! &
.MM& '
StateMM' ,
==MM- /
CommunicationStateMM0 B
.MMB C
OpenedMMC I
&&MMJ L
!MMM N
stringMMN T
.MMT U
IsNullOrEmptyMMU b
(MMb c
userToDisconnectMMc s
)MMs t
&&MMu w
!MMx y
stringMMy 
.	MM Ä
IsNullOrEmpty
MMÄ ç
(
MMç é
lobbyToDisconnect
MMé ü
)
MMü †
)
MM† °
{NN 
proxyOO 
.OO 
leaveLobbyChatAsyncOO )
(OO) *
userToDisconnectOO* :
,OO: ;
lobbyToDisconnectOO< M
)OOM N
;OON O
ConsolePP 
.PP 
	WriteLinePP !
(PP! "
$"PP" $
$strPP$ K
{PPK L
userToDisconnectPPL \
}PP\ ]
$strPP] a
{PPa b
lobbyToDisconnectPPb s
}PPs t
$strPPt ~
"PP~ 
)	PP Ä
;
PPÄ Å
}QQ 
trySS 
{TT 
ifUU 
(UU 
proxyUU 
!=UU 
nullUU !
)UU! "
{VV 
ifWW 
(WW 
proxyWW 
.WW 
StateWW #
==WW$ &
CommunicationStateWW' 9
.WW9 :
OpenedWW: @
||WWA C
proxyWWD I
.WWI J
StateWWJ O
==WWP R
CommunicationStateWWS e
.WWe f
OpeningWWf m
)WWm n
proxyWWo t
.WWt u
CloseWWu z
(WWz {
)WW{ |
;WW| }
elseXX 
proxyXX 
.XX 
AbortXX $
(XX$ %
)XX% &
;XX& '
ConsoleYY 
.YY 
	WriteLineYY %
(YY% &
$"YY& (
$strYY( U
"YYU V
)YYV W
;YYW X
}ZZ 
}[[ 
catch\\ 
(\\ 
	Exception\\ 
ex\\ 
)\\  
{]] 
Console^^ 
.^^ 
	WriteLine^^ !
(^^! "
$"^^" $
$str^^$ \
{^^\ ]
ex^^] _
.^^_ `
Message^^` g
}^^g h
"^^h i
)^^i j
;^^j k
proxy__ 
?__ 
.__ 
Abort__ 
(__ 
)__ 
;__ 
}`` 
finallyaa 
{bb 
proxycc 
=cc 
nullcc 
;cc 
sitedd 
=dd 
nulldd 
;dd 
callbackHandleree 
=ee  !
nullee" &
;ee& '
connectedUsernameff !
=ff" #
nullff$ (
;ff( )
connectedLobbyIdgg  
=gg! "
nullgg# '
;gg' (
}hh 
}ii 	
publickk 
boolkk 
IsConnectedkk 
(kk  
)kk  !
{ll 	
returnmm 
proxymm 
!=mm 
nullmm  
&&mm! #
proxymm$ )
.mm) *
Statemm* /
==mm0 2
CommunicationStatemm3 E
.mmE F
OpenedmmF L
;mmL M
}nn 	
}oo 
}pp §
èC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Services\ChatCallbackHandler.cs
	namespace 	
MindWeaveClient
 
. 
Services "
{ 
public 

class 
ChatCallbackHandler $
:% & 
IChatManagerCallback' ;
{		 
public

 
event

 
Action

 
<

 
ChatMessageDto

 *
>

* + 
LobbyMessageReceived

, @
;

@ A
public 
void 
receiveLobbyMessage '
(' (
ChatMessageDto( 6

messageDto7 A
)A B
{ 	
Debug 
. 
	WriteLine 
( 
$" 
$str H
{H I

messageDtoI S
.S T
senderUsernameT b
}b c
$strc f
{f g

messageDtog q
.q r
contentr y
}y z
$strz {
"{ |
)| }
;} ~
Application 
. 
Current 
?  
.  !

Dispatcher! +
?+ ,
., -
Invoke- 3
(3 4
(4 5
)5 6
=>7 9
{  
LobbyMessageReceived $
?$ %
.% &
Invoke& ,
(, -

messageDto- 7
)7 8
;8 9
} 
) 
; 
} 	
} 
} Ê
ñC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Helpers\StringToVisibilityConverter.cs
	namespace 	
MindWeaveClient
 
. 
Helpers !
{ 
public 

class '
StringToVisibilityConverter ,
:- . 
IMultiValueConverter/ C
{		 
public

 
object

 
Convert

 
(

 
object

 $
[

$ %
]

% &
values

' -
,

- .
Type

/ 3

targetType

4 >
,

> ?
object

@ F
	parameter

G P
,

P Q
CultureInfo

R ]
culture

^ e
)

e f
{ 	
if 
( 
values 
!= 
null 
&& !
values" (
.( )
Length) /
==0 2
$num3 4
&&5 7
values8 >
[> ?
$num? @
]@ A
isB D
stringE K
hostUsernameL X
&&Y [
values\ b
[b c
$numc d
]d e
isf h
stringi o"
currentPlayerUsername	p Ö
)
Ö Ü
{ 
return 
( 
! 
string 
.  
IsNullOrEmpty  -
(- .
hostUsername. :
): ;
&&< >
hostUsername? K
.K L
EqualsL R
(R S!
currentPlayerUsernameS h
,h i
StringComparisonj z
.z {
OrdinalIgnoreCase	{ å
)
å ç
)
ç é
? 

Visibility  
.  !
Visible! (
:) *

Visibility+ 5
.5 6
	Collapsed6 ?
;? @
} 
return 

Visibility 
. 
	Collapsed '
;' (
} 	
public 
object 
[ 
] 
ConvertBack #
(# $
object$ *
value+ 0
,0 1
Type2 6
[6 7
]7 8
targetTypes9 D
,D E
objectF L
	parameterM V
,V W
CultureInfoX c
cultured k
)k l
{ 	
throw 
new #
NotImplementedException -
(- .
). /
;/ 0
} 	
} 
} ⁄
ìC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Helpers\StringEquialityConverter.cs
	namespace 	
MindWeaveClient
 
. 
Helpers !
{ 
public 

class #
StringEqualityConverter (
:) *
IValueConverter+ :
{ 
public		 
object		 
Convert		 
(		 
object		 $
value		% *
,		* +
Type		, 0

targetType		1 ;
,		; <
object		= C
	parameter		D M
,		M N
CultureInfo		O Z
culture		[ b
)		b c
{

 	
string 
stringValue 
=  
value! &
as' )
string* 0
;0 1
string 
parameterString "
=# $
	parameter% .
as/ 1
string2 8
;8 9
return 
string 
. 
Equals  
(  !
stringValue! ,
,, -
parameterString. =
,= >
StringComparison? O
.O P
OrdinalIgnoreCaseP a
)a b
;b c
} 	
public 
object 
ConvertBack !
(! "
object" (
value) .
,. /
Type0 4

targetType5 ?
,? @
objectA G
	parameterH Q
,Q R
CultureInfoS ^
culture_ f
)f g
{ 	
throw 
new #
NotImplementedException -
(- .
). /
;/ 0
} 	
} 
} ı
¶C:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Helpers\StringEqualityToVisibilityConverterInverted.cs
	namespace 	
MindWeaveClient
 
. 
Helpers !
{ 
public 

class 7
+StringEqualityToVisibilityConverterInverted <
:= >
IValueConverter? N
{		 
public

 
object

 
Convert

 
(

 
object

 $
value

% *
,

* +
Type

, 0

targetType

1 ;
,

; <
object

= C
	parameter

D M
,

M N
CultureInfo

O Z
culture

[ b
)

b c
{ 	
string 
stringValue 
=  
value! &
as' )
string* 0
;0 1
string 
parameterString "
=# $
	parameter% .
as/ 1
string2 8
;8 9
bool 
areEqual 
= 
string "
." #
Equals# )
() *
stringValue* 5
,5 6
parameterString7 F
,F G
StringComparisonH X
.X Y
OrdinalIgnoreCaseY j
)j k
;k l
return 
areEqual 
? 

Visibility (
.( )
	Collapsed) 2
:3 4

Visibility5 ?
.? @
Visible@ G
;G H
} 	
public 
object 
ConvertBack !
(! "
object" (
value) .
,. /
Type0 4

targetType5 ?
,? @
objectA G
	parameterH Q
,Q R
CultureInfoS ^
culture_ f
)f g
{ 	
throw 
new #
NotImplementedException -
(- .
). /
;/ 0
} 	
} 
} å
ûC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Helpers\InverseBooleanToVisibilityConverter.cs
	namespace 	
MindWeaveClient
 
. 
Helpers !
{ 
[ 
ValueConversion 
( 
typeof 
( 
bool  
)  !
,! "
typeof# )
() *

Visibility* 4
)4 5
)5 6
]6 7
public		 

class		 /
#InverseBooleanToVisibilityConverter		 4
:		5 6
IValueConverter		7 F
{

 
public 
object 
Convert 
( 
object $
value% *
,* +
Type, 0

targetType1 ;
,; <
object= C
	parameterD M
,M N
CultureInfoO Z
culture[ b
)b c
{ 	
if 
( 
value 
is 
bool 
	boolValue '
)' (
{ 
return 
	boolValue  
?! "

Visibility# -
.- .
	Collapsed. 7
:8 9

Visibility: D
.D E
VisibleE L
;L M
} 
return 

Visibility 
. 
Visible %
;% &
} 	
public 
object 
ConvertBack !
(! "
object" (
value) .
,. /
Type0 4

targetType5 ?
,? @
objectA G
	parameterH Q
,Q R
CultureInfoS ^
culture_ f
)f g
{ 	
throw 
new #
NotImplementedException -
(- .
). /
;/ 0
} 	
} 
} ä
òC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Helpers\EqualityToVisibilityConverter.cs
	namespace 	
MindWeaveClient
 
. 
Helpers !
{ 
public 

class )
EqualityToVisibilityConverter .
:/ 0
IValueConverter1 @
{		 
public

 
object

 
Convert

 
(

 
object

 $
value

% *
,

* +
Type

, 0

targetType

1 ;
,

; <
object

= C
	parameter

D M
,

M N
CultureInfo

O Z
culture

[ b
)

b c
{ 	
bool 
areEqual 
= 
object "
." #
Equals# )
() *
value* /
?/ 0
.0 1
ToString1 9
(9 :
): ;
,; <
	parameter= F
?F G
.G H
ToStringH P
(P Q
)Q R
)R S
;S T
return 
areEqual 
? 

Visibility (
.( )
Visible) 0
:1 2

Visibility3 =
.= >
	Collapsed> G
;G H
} 	
public 
object 
ConvertBack !
(! "
object" (
value) .
,. /
Type0 4

targetType5 ?
,? @
objectA G
	parameterH Q
,Q R
CultureInfoS ^
culture_ f
)f g
{ 	
throw 
new #
NotImplementedException -
(- .
). /
;/ 0
} 	
} 
} ß
òC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Helpers\KickButtonVisibilityConverter.cs
	namespace 	
MindWeaveClient
 
. 
Helpers !
{ 
public 

class )
KickButtonVisibilityConverter .
:/ 0 
IMultiValueConverter1 E
{		 
public

 
object

 
Convert

 
(

 
object

 $
[

$ %
]

% &
values

' -
,

- .
Type

/ 3

targetType

4 >
,

> ?
object

@ F
	parameter

G P
,

P Q
CultureInfo

R ]
culture

^ e
)

e f
{ 	
if 
( 
values 
. 
Length 
==  
$num! "
&&# %
values& ,
[, -
$num- .
]. /
is0 2
bool3 7
isHost8 >
&&? A
valuesB H
[H I
$numI J
]J K
isL N
stringO U
playerUsernameV d
&&e g
valuesh n
[n o
$numo p
]p q
isr t
stringu {
hostUsername	| à
)
à â
{ 
if 
( 
isHost 
&& 
! 
string %
.% &
IsNullOrEmpty& 3
(3 4
playerUsername4 B
)B C
&&D F
!G H
playerUsernameH V
.V W
EqualsW ]
(] ^
hostUsername^ j
,j k
StringComparisonl |
.| }
OrdinalIgnoreCase	} é
)
é è
)
è ê
{ 
return 

Visibility %
.% &
Visible& -
;- .
} 
} 
return 

Visibility 
. 
	Collapsed '
;' (
} 	
public 
object 
[ 
] 
ConvertBack #
(# $
object$ *
value+ 0
,0 1
Type2 6
[6 7
]7 8
targetTypes9 D
,D E
objectF L
	parameterM V
,V W
CultureInfoX c
cultured k
)k l
{ 	
throw 
new #
NotImplementedException -
(- .
). /
;/ 0
} 	
} 
} ¶
åC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Helpers\PasswordBoxHelper.cs
	namespace 	
MindWeaveClient
 
. 
Helpers !
{ 
public 

static 
class 
PasswordBoxHelper )
{ 
public		 
static		 
readonly		 
DependencyProperty		 1
BoundPassword		2 ?
=		@ A
DependencyProperty

 
.

 
RegisterAttached

 /
(

/ 0
$str

0 ?
,

? @
typeof

A G
(

G H
string

H N
)

N O
,

O P
typeof

Q W
(

W X
PasswordBoxHelper

X i
)

i j
,

j k
new 
PropertyMetadata $
($ %
string% +
.+ ,
Empty, 1
,1 2"
onBoundPasswordChanged3 I
)I J
)J K
;K L
public 
static 
string 
GetBoundPassword -
(- .
DependencyObject. >
d? @
)@ A
{ 	
return 
( 
string 
) 
d 
. 
GetValue %
(% &
BoundPassword& 3
)3 4
;4 5
} 	
public 
static 
void 
SetBoundPassword +
(+ ,
DependencyObject, <
d= >
,> ?
string@ F
valueG L
)L M
{ 	
d 
. 
SetValue 
( 
BoundPassword $
,$ %
value& +
)+ ,
;, -
} 	
private 
static 
void "
onBoundPasswordChanged 2
(2 3
DependencyObject3 C
dD E
,E F.
"DependencyPropertyChangedEventArgsG i
ej k
)k l
{ 	
if 
( 
! 
( 
d 
is 
PasswordBox "
box# &
)& '
)' (
return) /
;/ 0
box 
. 
PasswordChanged 
-=  "
passwordChanged# 2
;2 3
string 
newValue 
= 
( 
string %
)% &
e& '
.' (
NewValue( 0
;0 1
if 
( 
box 
. 
Password 
!= 
newValue  (
)( )
{   
box!! 
.!! 
Password!! 
=!! 
newValue!! '
??!!( *
string!!+ 1
.!!1 2
Empty!!2 7
;!!7 8
}"" 
box$$ 
.$$ 
PasswordChanged$$ 
+=$$  "
passwordChanged$$# 2
;$$2 3
}%% 	
private&& 
static&& 
void&& 
passwordChanged&& +
(&&+ ,
object&&, 2
sender&&3 9
,&&9 :
RoutedEventArgs&&; J
e&&K L
)&&L M
{'' 	
if(( 
((( 
sender(( 
is(( 
PasswordBox(( %
box((& )
)(() *
{)) 
SetBoundPassword++  
(++  !
box++! $
,++$ %
box++& )
.++) *
Password++* 2
)++2 3
;++3 4
},, 
}-- 	
}.. 
}// Á
èC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Helpers\InverseBoolConverter.cs
	namespace 	
MindWeaveClient
 
. 
Helpers !
{ 
public 

class  
InverseBoolConverter %
:& '
IValueConverter( 7
{ 
public		 
object		 
Convert		 
(		 
object		 $
value		% *
,		* +
Type		, 0

targetType		1 ;
,		; <
object		= C
	parameter		D M
,		M N
CultureInfo		O Z
culture		[ b
)		b c
{

 	
if 
( 
value 
is 
bool 
	boolValue '
)' (
{ 
return 
! 
	boolValue !
;! "
} 
return 
false 
; 
} 	
public 
object 
ConvertBack !
(! "
object" (
value) .
,. /
Type0 4

targetType5 ?
,? @
objectA G
	parameterH Q
,Q R
CultureInfoS ^
culture_ f
)f g
{ 	
if 
( 
value 
is 
bool 
	boolValue '
)' (
{ 
return 
! 
	boolValue !
;! "
} 
return 
false 
; 
} 	
} 
} √
ìC:\Users\Dell\Documents\Materias Universidad\5to Semestre\construccion\Proyecto\MindWeaveClient\MindWeaveClient\Helpers\DifficultyIndexConverter.cs
	namespace 	
MindWeaveClient
 
. 
Helpers !
{ 
public 

class $
DifficultyIndexConverter )
:* +
IValueConverter, ;
{ 
public		 
object		 
Convert		 
(		 
object		 $
value		% *
,		* +
Type		, 0

targetType		1 ;
,		; <
object		= C
	parameter		D M
,		M N
CultureInfo		O Z
culture		[ b
)		b c
{

 	
if 
( 
value 
is 
int 
id 
&&  "
id# %
>& '
$num( )
)) *
return+ 1
id2 4
-5 6
$num7 8
;8 9
return 
$num 
; 
} 	
public 
object 
ConvertBack !
(! "
object" (
value) .
,. /
Type0 4

targetType5 ?
,? @
objectA G
	parameterH Q
,Q R
CultureInfoS ^
culture_ f
)f g
{ 	
if 
( 
value 
is 
int 
index "
&&# %
index& +
>=, .
$num/ 0
)0 1
return2 8
index9 >
+? @
$numA B
;B C
return 
$num 
; 
} 	
} 
} 