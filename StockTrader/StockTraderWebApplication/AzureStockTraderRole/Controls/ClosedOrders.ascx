<%@ Control Language="C#" AutoEventWireup="true" Inherits="Trade.Web.ClosedOrders" Codebehind="ClosedOrders.ascx.cs" %>
<%@ Import Namespace="Trade.StockTraderWebApplicationModelClasses" %>
<%@ Import Namespace="Trade.StockTraderWebApplicationSettings" %>
<table class="TradeHomeTableStyle" align="center">
<% if (closedOrderData!=null)
    if (closedOrderData.Count>0)
               {%> 
       <tr>    
            <td colspan="8">
            <center>
             <table class="OrderAlertTableStyle" style="background-image:url('Images/AlertFill.png');border:#50525b 2px solid">
             <tr >
                <td colspan="8" class="OrderAlertMessageStyle" style="border:#50525b 2px solid;font-weight:normal;font-size:1.1em">Trade Alert: The following orders have completed.</td>
             </tr>
             <tr>
             
                <th class="InnerHeading" >
                Order ID</th>
                <th class="InnerHeading">
                Order Status</th>
                <th class="InnerHeading">
                Creation Date</th>
                <th class="InnerHeading">
                Completion Date</th>
               <th class="InnerHeading">
                Txn Fee</th>
               <th class="InnerHeading">
                Type</th>
                <th class="InnerHeading">
                Symbol</th>
                <th class="InnerHeading">
                Quantity</th>
            </tr>     
            
            <%for (int i=0; i<closedOrderData.Count; i++)
                   { %>  
            <tr>
                <td class="InnerData">
                <%=closedOrderData[i].orderID%> </td>
                <td class="InnerData">
                <%=closedOrderData[i].orderStatus%></td>
                <td class="InnerData">
                <%=closedOrderData[i].openDate%></td>
                <td class="InnerData">
                <%=closedOrderData[i].completionDate%> </td>
                 <td class="InnerData" style="text-align:right">
                <%=closedOrderData[i].orderFee.ToString("C")%></td>
                <td class="InnerData">
                <%=closedOrderData[i].orderType%></td>
                <td class="InnerData">
                <%=closedOrderData[i].quoteLink%></td>
                 <td class="InnerData" style="text-align:right">
                <%=closedOrderData[i].quantity.ToString("N")%></td>
            </tr>      
            <%}%>
            </table>
            </center>
            <br />
            
       </td>
    </tr>
  <%  } %>
</table>    

<!-- End Order Alert Repeater Display ASP.NET User Control -->