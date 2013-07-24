<%@ Page Buffer="true" Title="About Us" Language="C#" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="false" AutoEventWireup="true" Inherits="About" Codebehind="About.aspx.cs" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <h2>
        About/Mobile
    </h2>
    <br />
    <table align="center" width="550px" >
    <tr>
    <td style="text-align:center;">
    .NET StockTrader 6.0.0.0 <br />
         Publication Date 9/24/2012<br /><span><a class="aboutlinks" href="http://social.msdn.microsoft.com/forums/en-US/dotnetstocktradersampleapplication/threads/">
         Support Link</a></span><br /><span><a class="aboutlinks" href="http://msdn.microsoft.com/stocktrader">
         MSDN Download Page</a></span><br /><br /><span style="font-weight: bold;">
         Mobile applications for Azure-connected Windows Phone, iOS and Android included with download!  Also includes a Windows 8 Modern UI front-end sample.  See application readme file for details.</span><br /><span><a class="aboutlinks" href="http://itunes.apple.com/us/app/azure-stocktrader-6.0/id557390977?mt=8&uo=4">
         iOS app in Apple App Store</a></span><br /><span><a class="aboutlinks" href="https://play.google.com/store/apps/details?id=stocktraderandroidclient.stocktraderandroidclient">
         Android app in Google Play Store</a></span>
    </td>
    </tr>
    <tr> 
         <td style="text-align:center;vertical-align:top;height:400px;">
         <asp:ImageButton ID="ConfigButton" runat="server" ImageUrl="~/Images/AboutBackground.jpg" BorderStyle="Ridge" BorderColor="Black" BorderWidth="3px" />
    </td>
    </tr>
    
    </table>
   <br />
</asp:Content>
