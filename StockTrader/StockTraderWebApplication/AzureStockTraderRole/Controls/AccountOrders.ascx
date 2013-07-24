<%@ Control Language="C#" AutoEventWireup="true" Inherits="Trade.Web.AccountOrders" Codebehind="AccountOrders.ascx.cs" %>
<!-- Start Order Alert ASP.NET User Control --> 

<asp:Repeater id="AccountOrdersRepeater" runat="server">
          <HeaderTemplate>
             <table class="OrderAlertTableStyle" width="710px" align="center" style="background-image:url('Images/QuotesFill.png');">
            <tr>
                <th class="InnerHeading">
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
                <th class="InnerHeading">
                Price</th>
                <th class="InnerHeading">
                Total</th>
            </tr>       
          </HeaderTemplate>
          <ItemTemplate>
            <tr>
                <td class="InnerData">
                <%# Eval("orderID")%> </td>
                <td class="InnerData">
                <%# Eval("orderStatus") %></td>
                <td class="InnerData">
                <%# Eval("openDate") %></td>
                <td class="InnerData">
                <%# Eval("completionDate") %> </td>
                 <td class="InnerData" style="text-align:right">
                <%# Eval("orderFee", "{0:C}")%></td>
                <td class="InnerData">
                <%# Eval("orderType")%></td>
                <td class="InnerData">
                <%# Eval("quoteLink")%></td>
                <td class="InnerData" style="text-align:right">
                <%# Eval("quantity","{0:0,0}")%></td>
                 <td class="InnerData" style="text-align:right">
                <%# Eval("price","{0:C}")%></td>
                 <td class="InnerData" style="text-align:right">
                <%# Eval("total","{0:C}")%></td>
           </tr>      
         </ItemTemplate>
         <FooterTemplate>
       </table>   
       </FooterTemplate>
</asp:Repeater>
<!-- End Order Alert ASP.NET User Control -->