<%@ Register TagPrefix="controls" TagName="ClosedOrders" Src = "Controls/ClosedOrders.ascx"  %>
<%@ Page Buffer="true" Title="Stock Quotes" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="Trade.Web.Quotes" Codebehind="Quotes.aspx.cs" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
   
    <h2>
        Quotes
    </h2>
    <br />
    <controls:ClosedOrders id="ClosedOrdersControl" runat="server" ></controls:ClosedOrders>
    
   <table class="QuotesTableStyle" align="center" style="background-image:url('Images/QuotesFill.png');">
             <tr>
                <th class="InnerHeading">
                Symbol</th>
                <th class="InnerHeading">
                Company</th>
                <th class="InnerHeading">
                Volume</th>
                <th class="InnerHeading">
                Price Range</th>
                <th class="InnerHeading">
                Open Price</th>
                <th class="InnerHeading">
                Current Price</th>
                <th class="InnerHeading">
                Gain(Loss)</th>
                <th class="InnerHeading">
                Trade</th>
           </tr>       
       
       <% if (quoteList !=null)
          {
              for (int i = 0; i < quoteList.Count; i++)
              {
       %>
          <tr>
                <td class="InnerData" style="text-align:center;">
                <%=quoteList[i].quoteLink%></td>
                <td class="InnerData" style="text-align:center;">
                <%=quoteList[i].companyName%> </td>
                <td class="InnerData" style="text-align:right">
                <%=quoteList[i].volume.ToString("N")%></td>
                <td class="InnerData" style="text-align:right">
                <%=quoteList[i].low.ToString("C")%> -  <%=quoteList[i].high.ToString("C")%> </td>
                <td class="InnerData" style="text-align:right">
                <%=quoteList[i].open.ToString("C")%></td>
                <td class="InnerData" style="text-align:right">
                <%=quoteList[i].price.ToString("C")%></td>
                <td class="InnerData" style="text-align:right">
                <%=quoteList[i].gainWithArrow%></td>
                <td class="InnerData" style="text-align:center;">
                <a href="StockTrade.aspx?action=buy&symbol=<%=quoteList[i].symbol%>">Buy</a>
                </td>
		 </tr>
		 <%}
 }%>


</table>
<center>
 <table width="700" align="center">
 <tr>
<td colspan="8" align="center">
<br />
<br />
 <hr style="vertical-align:bottom;"/></td>
</tr>
    <tr>
        <td style="width:400px;height:30px;text-align:right;">
            <asp:TextBox ID="Symbols" CssClass="textEntry" runat="server" Text="s:100;s:101;s:102;s:103" Width="150px"></asp:TextBox>
        </td>
        <td style="padding-left:10px;padding-bottom:4px;text-align:left;">
            <asp:Button ID="QuoteButton" runat="server"  CommandName="Quotes" 
                Text="Get Quotes" Width="100px" onclick="QuoteButton_Click" />
        </td>
    </tr>
</table> 
       </center>

    <br />
    
</asp:Content>

