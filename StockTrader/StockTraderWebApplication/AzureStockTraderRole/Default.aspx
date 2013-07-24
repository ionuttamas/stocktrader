<%@ Page Buffer="true" Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="Trade.Web._Default" Codebehind="Default.aspx.cs" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
   
    <h2>
        Welcome to the Azure StockTrader!
    </h2>
    <ul>
        <li style="text-align: left"><a href="login.aspx">
            Go Trade!</a>
        </li>
            <li><a href="https://azurestocktrader.blob.core.windows.net/docs/Trade6Overview.pdf" target="_blank">StockTrader 6 Technical Overview</a></li>
       
        <li style="text-align: left">
            <a href="https://azurestocktrader.blob.core.windows.net/docs/StockTraderOverview.ppsx"  target="_blank">Slideshow Presentation</a></li>
            <li style="text-align: left">
            <a href="https://azurestocktrader.blob.core.windows.net/docs/stocktraderconfiguration.pdf"  target="_blank">Step-by-Step Configuration Guide</a></li>
            <li><a href="Docs.aspx">Complete Documentation Listing</a></li>
        </ul>
        <table>
        <tr>
        <td width="500px">
        <p>
        This application is an end-to-end sample application for 
            <a href="http://www.microsoft.com/windowsazure/windowsazure/">Windows Azure</a> and Microsoft cloud technologies.
        It is a service-oriented application based on the .NET Framework 4.0, including ASP.NET and Windows Communication Foundation (WCF).
        It illustrates many of the .NET enterprise development technologies that can be used for building highly scalable, "cloud-connected" applications. 
        The application demonstrates a single code base that works on traditional servers, Windows Server Hyper-V private clouds, and in the Windows Azure public 
        cloud. Make sure to visit <asp:HyperLink ID="HyperLinkConfigWeb" runat="server">ConfigWeb</asp:HyperLink> to explore the deployment.
        </p>
        <p>
            Each element of the application is designed as an autonomous service, including 
            the Web application front-end, the Business Service middle tier, and the Order 
            Processing Service. Each can be deployed on premise, or on the Windows Azure 
            cloud. Each element, from a single code-base, can work with on-premise SQL 
            Server databases, or with the SQL Azure relational database service. 
        </p>
        <p>
        <a href="http://msdn.microsoft.com/stocktrader">Check back</a> on MSDN for periodic
        updates to this sample based on community feedback, bug fixes, and enhancements.
        </p>
        </td>       
        <td style="padding-left:35px;vertical-align:top;padding-top:16px"">
            
            <table class="technologies" align="center" style="background-image:url('images/techtablefill.png')">
            <tr>
                <th style="text-align:center; vertical-align:middle;font-size:1.2em;font-weight:normal;background-color:#112e58; color:white; height: 25px;">
                Sample of Technologies Demonstrated</th>
            </tr>
            <tr >
            <td style="padding-right:8px;">   
            <ul>
            <li class="Mainline">
                Integrating private and public clouds    
                </li>
                <li class="Subline">
                Windows Azure and Windows Server    
                </li>
                <li class="Subline">
                    SQL Azure and SQL Server</li>
                <li class="Subline">
                Windows Azure AppFabric and Windows Server AppFabric</li>
                <li class="Mainline">
                Service-oriented, n-tier design with ASP.NET and WCF    
                </li>
                <li class="Subline">
                Clean separation of UI, business services and DB access    
                </li>
                <li class="Subline">
                Design and tuning for performance   
                </li>
                <li class="Subline">
                Horizontally scalable on Windows Azure and on-premise</li>
                <li class="Subline">
                Centralized configuration of clustered service nodes   
                </li>
                <li class="Subline">
                Alternative configurations for performance comparisons </li>
                <li class="Mainline">
                .NET Framework 4.0/C#
                </li>
                <li class="Subline">
                ASP.NET 4.0
                </li>
                <li class="Subline">
                WCF 4.0 with service security
                </li>
                <li class="Subline">
                ADO.NET 4.0
                </li>
                
            </ul>
            </td>
         </tr>
         </table>
         <br />
        </td>
        </tr>
        </table>
     
       <center><asp:Image ID="TradeImage" runat="server" ImageUrl="~/Images/trade.gif" /><br /></center>
   <br />

    
</asp:Content>

