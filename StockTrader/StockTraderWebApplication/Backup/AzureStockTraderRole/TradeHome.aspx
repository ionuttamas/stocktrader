<%@ Register TagPrefix="controls" TagName="MarketSummary" Src = "Controls/MarketSummary.ascx"  %>
<%@ Register TagPrefix="controls" TagName="ClosedOrders" Src = "Controls/ClosedOrders.ascx"  %>
<%@ Page  Buffer="true" Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="Trade.Web.TradeHome" Codebehind="TradeHome.aspx.cs" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    
    <h2>
        Trade Home
    </h2>
    <br />
    <controls:ClosedOrders id="ClosedOrdersControl" runat="server" ></controls:ClosedOrders>
    <table align="center">
<tr>
    <td colspan = "3" class="NameStyle">Welcome, <asp:Label id="Name" runat="server"></asp:Label>
    <br /><br /></td>
</tr>
<tr>
            <td colspan="2" class="SubHeaderStyle2">
            <table width="500px" align="center">
               <tr>
                  <td colspan="4" style="padding-top:7px;padding-left:60px;text-align:left"> User Statistics
                  <hr style="width:350px;text-align:left;"/>  </td>
               </tr>
               <tr>
                  <td class="DataHeadersStyle">
                  <a href="Glossary.aspx">Account ID:</a></td>
                  <td class="DataLabelsStyle"> 
                  <asp:Label ID="AccountID" runat="server"></asp:Label></td>
               </tr>
               <tr>
                  <td class="DataHeadersStyle">
                  <a href="Glossary.aspx">Account Created:</a></td>
                  <td class="DataLabelsStyle"> 
                  <asp:Label ID="CreationDate" runat="server"></asp:Label></td>
               </tr>
               <tr>
                  <td class="DataHeadersStyle">
                  <a href="Glossary.aspx">Total Logins:</a></td>
                  <td class="DataLabelsStyle"> 
                  <asp:Label ID="LoginCount" runat="server"></asp:Label></td>
               </tr> 
               <tr> 
                  <td class="DataHeadersStyle">
                  <a href="Glossary.aspx">Session Created:</a></td>
                  <td class="DataLabelsStyle"> 
                  <asp:Label ID="SessionCreateDate" runat="server"></asp:Label>
                  </td>
               </tr>
               <tr>
                  <td class="DataHeadersStyle"><br /></td>
               </tr>
               <tr>
                  <td colspan="4" style="padding-top:7px;padding-left:60px;">
                  Account Summary
                   <hr style="width:350px;text-align:left;"/>  </td>
               </tr>
               <tr>
                  <td class="DataHeadersStyle">
                  <a href="Glossary.aspx"> Cash Balance:</a></td>
                  <td class="DataLabelsStyle"> 
                  <asp:Label ID="Balance" runat="server"></asp:Label></td>
               </tr>
               <tr>
                  <td class="DataHeadersStyle">
                  <a href="Glossary.aspx">Number of Holdings:</a></td>
                  <td class="DataLabelsStyle"> 
                  <asp:Label ID="NumHoldings" runat="server"></asp:Label></td>
               </tr>
               <tr>
                  <td class="DataHeadersStyle">
                  <a href="Glossary.aspx">Total of Holdings: </a></td>
                  <td class="DataLabelsStyle"> 
                  <asp:Label ID="HoldingsTotal" runat="server"></asp:Label></td>  
               </tr>
               <tr>
                  <td class="DataHeadersStyle">
                  <a href="Glossary.aspx">Sum of Cash & Holdings</a></td>
                  <td class="DataLabelsStyle"> 
                  <asp:Label ID="SumOfCashHoldings" runat="server"></asp:Label></td> 
               </tr>
               <tr>
                  <td class="DataHeadersStyle">
                  <a href="Glossary.aspx">Opening Balance:</a></td>
                  <td class="DataLabelsStyle"> 
                  <asp:Label ID="OpenBalance" runat="server"></asp:Label></td> 
               </tr>
               <tr>
                  <td class="DataHeadersStyle">
                  <a href="Glossary.aspx">Current Gain/(Loss):</a></td>
                  <td class="DataLabelsStyle"> 
                  <asp:Label ID="Gain" runat="server"></asp:Label></td> 
               </tr>
               <tr>
                  <td class="DataHeadersStyle">
                  <a href="Glossary.aspx">% Gain/(Loss):</a></td>
                  <td class="DataLabelsStyle"> 
                  <asp:Label ID="PercentGain" runat="server"></asp:Label></td> 
               </tr>
            </table></td>
            <td style="text-align:center;">
            <center>
<!-- Start MarketSummary ASP.NET User Control -->
 <controls:MarketSummary id="MarketSummary" runat="server" ></controls:MarketSummary>
           </center>
<!-- End MarketSummary ASP.NET User Control -->   
            </td>
         </tr>
        <tr>
            <td><br /></td>
        </tr>
   
<tr>
<td colspan="4" align="center"> <hr style="vertical-align:bottom"/></td>
</tr>
 <tr>
 <td colspan="4" align="center" >

 <table style="width:600" align="center">
    <tr>
        <td style="width:100px;height:30px;text-align:right">
            <asp:TextBox ID="symbols" CssClass="textEntry" runat="server" Text="s:100;s:101;s:102;s:103" Width="150px"></asp:TextBox>
        </td>
        <td style="padding-left:10px;padding-bottom:4px;">
            <asp:Button ID="QuoteButton" runat="server"  CommandName="Quotes" 
                Text="Get Quotes" Width="100px" onclick="QuoteButton_Click" />
        </td>
    </tr>
</table> 
       
</td></tr>
</table>
    <br />
</asp:Content>

