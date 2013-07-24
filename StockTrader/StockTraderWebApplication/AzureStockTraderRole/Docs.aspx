<%@ Page Buffer="true" Title="Documentation" Language="C#" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="false" AutoEventWireup="true" Inherits="Trade.Web.Docs" Codebehind="Docs.aspx.cs" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <h2>
        Documentation
    </h2>
         <table>
        <tr>
        <td width="900px">
        <p>
        StockTrader 6.0 is an end-to-end sample application with a variety of different 
            services and corresponding Visual Studio projects.  This page is meant to
        highlight the key elements of the sample and how they can be used.  New with the 5.0 version is a Visual Studio Template and Solution Wizard that 
        templatizes the StockTrader 5.0 design pattern to jumpstart custom solutions implementing this design pattern. This is named the <b>Config Service 5</b> template, and
        will show up as a C# project type.  Besides providing a turnkey implementation of the Configuration Service, it provides a UI 
            Layer,
        Business Service Layer (BSL), and Data Access Layer (DAL) ala StockTrader 6.0.  The wizard lets you select your host environment, including on-premise or Azure projects, so you
        should check this out.  Of course, the StockTrader 6.0 application demonstrates just one of several enterprise design patterns possible, in this case a multi-tier application
        designed for very fast performance and horizontal scale out. There is not, however, <b>one</b> design pattern that fits every organization's needs (and project needs).
        So other design patterns are possible and we hope to extend StockTrader to illustrate these as well, 
            over time: for example an ASP.NET MVC
        (vs. ASP.NET Web Forms) application with an ADO.NET Entity Framework DAL; and/or a Business Service tier that uses WCF RIA Services and OData to work with a mobile device client running on
         any mobile device running HTML 5.  All of these are possible, and based on the logical partitioning of the application, can be achieved with good code re-use.
         </p>
         <p>
         If you use the ConfigService 5.0 template, keep in mind the Configuration Service itself does not impose any particular design pattern or coding style on your application--you can substitute
         your favorite design pattern for the provided UI, BSL and DAL tiers. And you might simply want to the use the UI, BSL and DAL template projects and not implement the Configuration Service--this is
         possible too, just by simple modifications to the template solution produced. The template can help produce a structured solution that either hosts services, is a client to 
             remote services, or both.
         Also, you can use it to create standalone ASP.NET Web applications for on-premise or Windows Azure that do not invoke 
             any remote services, but utilize the BSL and DAL as logical, structured tiers 
             within the Web application.</p>
        <p>
        Below is a list of key documents for the sample.  You don't have to read all of them to use the sample!
        </p>
        <ul>
        <li>
        <a href="https://azurestocktrader.blob.core.windows.net/docs/GuideToSolutions.pdf" target="_blank">Guide to Visual Studio Solutions</a>
        </li>
         <li>
        <a href="https://azurestocktrader.blob.core.windows.net/docs/Trade6Overview.pdf" target="_blank">StockTrader 6.0 Technical Overview</a>
        </li>
         <li>
        <a href="https://azurestocktrader.blob.core.windows.net/docs/StockTraderOverview.ppsx" target="_blank">StockTrader 6.0 SlideShow</a>
        </li>
        <li>
        <a href="https://azurestocktrader.blob.core.windows.net/docs/stocktraderconfiguration.pdf" target="_blank">Tutorial: Changing the StockTrader 6.0 Configuration and Using ConfigWeb</a>
        </li>
        <li>
        <a href="https://azurestocktrader.blob.core.windows.net/docs/ConfigServiceVSTemplate.pdf" target="_blank">Using the Visual Studio Template/Implementing Configuration Service 6.0 and the StockTrader 6.0 Design Pattern</a>
        </li>
        <li>
        <a href="https://azurestocktrader.blob.core.windows.net/docs/ConfigServiceTechnicalGuide.pdf" target="_blank">Configuration Service 6.0 Technical Guide</a>
        </li>
        </ul>
        
        <p>You may also be interested in learning more about Windows Azure and SQL Azure.  Below are two presentations on each cloud technology:</p>
        <ul>
        <li>
        <a href="https://azurestocktrader.blob.core.windows.net/docs/WindowsAzure.ppsx" target="_blank">Windows Azure Technical Overview</a>
        </li>
         <li>
        <a href="https://azurestocktrader.blob.core.windows.net/docs/SQLAzure.ppsx" target="_blank">SQL Azure Technical Overview</a>
        </li>
        </ul>
        <p>
        Here are some key facts about StockTrader 6.0:
        </p>
        <ul>
        <li>The StockTrader sample can be used against SQL Azure.</li>
        <li>The application is logically partitioned into UI, Business Service (BSL), and Data Access (DAL) tiers. </li>
        <li>The application can be run in remote modes using WCF services, and also be run in "collapsed" mode as a <b>standalone</b> Azure Web Application.</li>
        <li>The ConfigWeb application is used to quickly change the modes (remote, or in-process for collapsed mode) the Web Tier, Business Service Layer, and Order Processor 
            use to operate.</li>
        <li>There is an additional UI tier built using Windows Presentation Foundation that runs as a desktop smart client application, 
            and communicates 
            with the remote BSL tier for all operations.
        This smart client desktop application can connect up to the StockTrader Busniess Services on-premise, or running in the Windows Azure cloud. Check it out as just one example of a hybrid cloud application.
        </li>
            <li>There are three core Microsoft technologies for creating hybrid cloud solutions 
                at the application layer: <b><a href="http://msdn.microsoft.com/en-us/netframework/aa663324.aspx" target="_blank">Secure WCF Services</a></b> as shown with StockTrader 6.0; 
                <b><a href="http://msdn.microsoft.com/en-us/library/gg433122.aspx" target="_blank">Windows Azure Connect</a></b>; and <b><a href="http://msdn.microsoft.com/en-us/library/ee732537.aspx" target="_blank">Windows Azure AppFabric ServiceBus</a></b>. All are 
                appropriate for different scenarios and different organizational requirements.</li>
        <li>There is a turnkey Capacity Planner tool that can be used to benchmark/stress the StockTrader BSL layer, and judge the capacity supported on different hardware environments including private
        and public clouds, or hybrid deployments.  If running the load-test agents against public cloud deployments, you will want to understand your outbound Internet connections and limitations
        that proxy servers and ISP connections may impose that could limit your ability to fully saturate the 
        cloud application with load.</li>
         <li>The ConfigWeb application provides a ServiceMap view that can be conveniently used to see several key performance statistics of the application and all its tiers as it runs in the cloud (or on-pemise) under load. This
         view looks at individual nodes, as well as a summary provided for the entire domain across all nodes running.&nbsp; 
             This view also highlights the online/offline status of nodes, databases, 
             distributed caches and any individual WCF service endpoints.</li>
            <li>The ConfigWeb application provides a <strong>Service Logs</strong> view that 
                shows exception and trace messages for all elements of the application. 
                Developers should use this view as they develop and debug; and monitor the log 
                for deployed applications to understand any issues being encountered for a live 
                deployment.</li>
        </ul>
        
        </td> 
        </tr>
        </table>
        <br />
        
        <br />
            
   
</asp:Content>
