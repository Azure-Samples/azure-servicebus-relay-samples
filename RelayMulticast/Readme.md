#NetEvent Sample

This sample demonstrates using the **NetEventRelayBinding** binding on the 
Azure Service Bus. This binding allows multiple applications to listen to events sent 
to an endpoint; events sent to that endpoint are received by all 
applications.

##Prerequisites
If you haven't already done so, read the release notes document that 
explains how to sign up for a Azure account and how to configure your 
environment. It also contains important information about the default security 
settings for your environment that you need to be aware of.

##Service Contract &amp; Implementation
This sample implements a chatroom via the project's<CODE> IMulticastContract</CODE> and <CODE>MulticastService</CODE> 
implementations. <CODE>Hello</CODE> and <CODE>Bye</CODE> are used within the chatroom application to 
notify participants when a user joins and leaves the chat. <CODE>Chat</CODE> is 
called by the application when a user provides a string to contribute to the 
conversation.

Note that the methods are and must be marked as <CODE>IsOneWay=True</CODE>. 
```C#
[ServiceContract(Name = "IMulticastContract", Namespace = "")]
public interface IMulticastContract
{
     [OperationContract(IsOneWay=true)]
     void Hello(string nickName);
 
     [OperationContract(IsOneWay = true)]
     void Chat(string nickName, string text);
 
     [OperationContract(IsOneWay = true)]
     void Bye(string nickName);
}
```

The service implementation is shown below.
```C#
[ServiceBehavior(Name = "MulticastService", Namespace = "http://samples.microsoft.com/ServiceModel/Relay/")]
class MulticastService : IMulticastContract
{
    void IMulticastContract.Hello(string nickName)
    {
        Console.WriteLine("[" + nickName + "] joins");
    }
 
    void IMulticastContract.Chat(string nickName, string text)
    {
        Console.WriteLine("[" + nickName + "] says: " + text);
    }
 
    void IMulticastContract.Bye(string nickName)
    {
        Console.WriteLine("[" + nickName + "] leaves");
    }   
}
```
##Configuration
<DIV id=sectionSection2 class=section><content 
xmlns="http://ddue.schemas.microsoft.com/authoring/2003/5">
<P xmlns="">The service and client endpoints use the **NetEventRelayBinding** 
binding. 
<DIV class=code xmlns=""><SPAN codeLanguage="xml">
<TABLE cellSpacing=0 cellPadding=0 width="100%">
  <TBODY>
  <TR>
    <TH>Xml&nbsp;</TH>
</TR>
  <TR>
    <TD colSpan=2><PRE>&lt;netEventRelayBinding&gt;
     &lt;binding name="default" /&gt;
&lt;/netEventRelayBinding&gt; 
</PRE></TD></TR></TBODY></TABLE></SPAN></DIV>
<P xmlns="">The endpoints for the service and client are defined in the 
application configuration file. The client address is a placeholder that is 
replaced in the application. The following endpoints are defined:
<DIV class=code xmlns=""><SPAN codeLanguage="xml">
<TABLE cellSpacing=0 cellPadding=0 width="100%">
  <TBODY>
  <TR>
    <TH>Xml&nbsp;</TH>
</TR>
  <TR>
    <TD colSpan=2><PRE>&lt;service name="Microsoft.ServiceBus.Samples.MulticastService"&gt;
    &lt;endpoint name="RelayEndpoint"
              contract="Microsoft.ServiceBus.Samples.IMulticastContract"
              binding="netEventRelayBinding"
              bindingConfiguration="default"
              address="" /&gt;
&lt;/service&gt;
 
&lt;client&gt;
    &lt;endpoint name="RelayEndpoint"
              contract="Microsoft.ServiceBus.Samples.IMulticastContract"
              binding="netTcpRelayBinding"
              bindingConfiguration="default"
              address="http://AddressToBeReplacedInCode/" /&gt;
&lt;/client&gt;</PRE></TD></TR></TBODY></TABLE></SPAN></DIV></content></DIV>
<H2 class=heading>Application</H2>
<DIV id=sectionSection3 class=section><content 
xmlns="http://ddue.schemas.microsoft.com/authoring/2003/5">
<P xmlns="">The application begins by obtaining the chat session name, the service 
namespace, the 
issuer credentials and a chat nickname (a string used to identify the 
chatter). The sample constructs the service URI using this information, then 
opens the service and the client channel to the Service Bus rendezvous endpoint 
for the chat session. The <CODE>Hello</CODE> method notifies all participating 
applications of the arrival of a new user. The <CODE>Chat</CODE> method sends all strings as messages to 
all participating applications until an empty string is provided as input. 
    At that point the client leaves the chatroom and the <CODE>Bye</CODE> method notifies all participants of the client&#39;s departure.
<DIV class=code xmlns=""><SPAN codeLanguage="CSharp">
<TABLE cellSpacing=0 cellPadding=0 width="100%">
  <TBODY>
  <TR>
    <TH>C#&nbsp;</TH>
</TR>
  <TR>
    <TD colSpan=2><PRE class="style2">Console.Write(&quot;What do you want to call your chat session? &quot;);
string session = Console.ReadLine();
Console.Write(&quot;Your Service Namespace: &quot;);
string serviceNamespace = Console.ReadLine();
Console.Write(&quot;Your Issuer Name: &quot;);
string issuerName = Console.ReadLine();
Console.Write(&quot;Your Issuer Secret: &quot;);
string issuerSecret = Console.ReadLine();
Console.Write(&quot;Your Chat Nickname: &quot;);
string chatNickname = Console.ReadLine();</PRE>
	<PRE>TransportClientEndpointBehavior relayCredentials = new TransportClientEndpointBehavior();
relayCredentials.TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerSecret);    

Uri serviceAddress = ServiceBusEnvironment.CreateServiceUri(&quot;sb&quot;, &quot;ChatRooms&quot;,
String.Format(CultureInfo.InvariantCulture, &quot;{0}/MulticastService/&quot;, session));
ServiceHost host = new ServiceHost(typeof(MulticastService), serviceAddress);
host.Description.Endpoints[0].Behaviors.Add(relayCredentials);
host.Open();

ChannelFactory&lt;IMulticastChannel&gt; channelFactory = new ChannelFactory&lt;IMulticastChannel&gt;(&quot;RelayEndpoint&quot;, new EndpointAddress(serviceAddress));
channelFactory.Endpoint.Behaviors.Add(relayCredentials);
IMulticastChannel channel = channelFactory.CreateChannel();
channel.Open();

Console.WriteLine(&quot;\nPress [Enter] to exit\n&quot;);

channel.Hello(chatNickname);

string input = Console.ReadLine();
while (input != String.Empty)
{
   channel.Chat(chatNickname, input);
   input = Console.ReadLine();
}

channel.Bye(chatNickname);

channel.Close();
channelFactory.Close();
host.Close();
</PRE></TD></TR></TBODY></TABLE></SPAN></DIV>
				</content></DIV>
<H2 class=heading>Building and Running the Sample</H2>
<DIV id=sectionSection4 class=section><content 
xmlns="http://ddue.schemas.microsoft.com/authoring/2003/5">
<P xmlns="">After building the solution, perform the following steps to run the 
application:
<OL class=ordered xmlns="">
  <LI>From a command prompt, run the application 
  bin\Debug\MulticastSample.exe.<BR><BR>
  <LI>From another command prompt, run another instance of the application 
  bin\Debug\MulticastSample.exe.<BR><BR></LI></OL>
<P xmlns="">When finished, press ENTER to exit the application.
<P xmlns="">**Expected Output – Application 1**
<DIV class=code xmlns=""><SPAN codeLanguage="other">
<TABLE cellSpacing=0 cellPadding=0 width="100%">
  <TBODY>
  <TR>
    <TD colSpan=2><PRE class="style1">What do you want to call your chat session? &lt;chat-session&gt;
Your Service Namespace: &lt;service-namespace&gt;
Your Issuer Name: owner
Your Issuer Secret: &lt;issuer-secret&gt;
Your Chat Nickname: &lt;chat-nickname&gt;

Press [Enter] to exit

[jill] joins
hello
[jill] says: hello
[jack] says: hi, how are you?
[jack] says: who do you think will win the superbowl this year?
</PRE></TD></TR></TBODY></TABLE></SPAN></DIV>
<P xmlns="">**Expected Output – Application 2**
<DIV class=code xmlns=""><SPAN codeLanguage="other">
<TABLE cellSpacing=0 cellPadding=0 width="100%">
  <TBODY>
  <TR>
    <TD colSpan=2><PRE class="style1">What do you want to call your chat session? &lt;chat-session&gt;
Your Service Namespace: &lt;service-namespace&gt;
Your Issuer Name: owner
Your Issuer Secret: &lt;issuer-secret&gt;
Your Chat Nickname: &lt;chat-nickname&gt;

Press [Enter] to exit

[jack] joins
[jill] joins
[jill] says: hello
hi, how are you?
[jack] says: hi, how are you?
who do you think will win the superbowl this year?
[jack] says: who do you think will win the superbowl this year?</PRE></TD></TR></TBODY></TABLE></SPAN></DIV></content></DIV><!--[if gte IE 5]><tool:tip 
avoidmouse="false" element="languageFilterToolTip"></tool:tip><![endif]--></DIV>

    <br /> 
    <hr /> 
    Did you find this information useful?
    <a href="http://go.microsoft.com/fwlink/?LinkID=155664">
      
      <linkText>
        Please send your suggestions and comments about the documentation.
      </linkText>
    </a>
</DIV></BODY></HTML>
