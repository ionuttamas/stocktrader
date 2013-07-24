<%@ Register TagPrefix="controls" TagName="ClosedOrders" Src = "Controls/ClosedOrders.ascx"  %>
<%@ Page Buffer="true" Title="Portfolio Listing by Holding" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="Trade.Web.PortfolioBySymbol" Codebehind="PortfolioBySymbol.aspx.cs" %>
<%@ Import Namespace="System.Text"%>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Portfolio Summary By Holding
    </h2>
    <br />
     <controls:ClosedOrders id="ClosedOrdersControl" runat="server" ></controls:ClosedOrders>
    <table align="center" width="800px">
            <tr>
             <td style="text-align:left;">Current Number of Unique Stocks Held:&nbsp<b><asp:Label ID="numOfUniqueStocks" runat="server"
             Text="numOfHoldings"></asp:Label></b></td> <td style="text-align:right;font-weight:normal;"><a href="Portfolio.aspx">Sort By Purchase Date</a></td>
            </tr>
     </table>
        <asp:Repeater id="PortfolioBySymbolRepeater" runat="server">
        <HeaderTemplate>
            <table class="PortfolioTableStyle" align="center" width="800px"  style="background-image:url('Images/PortfolioFill.png');">
             <tr>
                <th class="InnerHeading">
                Holding ID</th>
                <th class="InnerHeading">
                Purchase<br />Date</th>
                <th class="InnerHeading">
                Symbol</th>
                <th class="InnerHeading">
                Quantity</th>
                <th class="InnerHeading">
                Purchase<br />Price</th>
                <th class="InnerHeading">
                Current<br />Price</th>
                <th class="InnerHeading">
                Purchase<br />Basis</th>
                <th class="InnerHeading">
                Market<br />Value</th>
                <th class="InnerHeading">
                Gain(Loss)</th>
                <th class="InnerHeading">
                Trade</th>
             </tr>       
       </HeaderTemplate>
       <ItemTemplate>
          <tr>
                <%# Eval("holdingID")%> 
                <%# Eval("purchaseDate") %>
                <%# Eval("quoteID")%>
                <%# Eval("quantity", "{0:0,0}") %>
                <%# Eval("purchasePrice", "{0:C}")%>
                <%# Eval("quotePrice", "{0:C}")%>
                <%# Eval("basis", "{0:C}")%>
                <%# Eval("marketValue","{0:C}")%>
                <%# Eval("gainWithArrow") %>
                <%# Eval("sellLink") %>
        </tr>      
      </ItemTemplate>
      <FooterTemplate>
        <tr>
                <th class="InnerHeading" style="text-align:right" colspan="6">Totals</th>
                <th class="InnerHeading" style="text-align:right"><%=String.Format("{0:C}",totalHoldings.basis) %></th>
                <th class="InnerHeading" style="text-align:right"><%=String.Format("{0:C}",totalHoldings.marketValue)%></th>
                <th class="InnerHeading" style="text-align:right; padding-right:3px;"><%=String.Format("{0:C}",totalHoldings.gainWithArrow)%></th>
                <th class="InnerHeading"></th>
        </tr>
        </table>
      </FooterTemplate>
      </asp:Repeater>
       <br />
      <hr style="vertical-align:bottom"/>
      <table style="width:600" align="center">
    <tr>
        <td style="width:100px;height:30px;text-align:right">
            <asp:TextBox ID="symbols" CssClass="textEntry" runat="server" 
                Text="s:100;s:101;s:102;s:103" Width="150px" TabIndex="1"></asp:TextBox>
        </td>
        <td style="padding-left:10px;padding-bottom:4px;">
            <asp:Button ID="QuoteButton" runat="server"  CommandName="Quotes" 
                Text="Get Quotes" Width="100px" onclick="QuoteButton_Click" TabIndex="2"/>
        </td>
    </tr>
    </table>
    <br />
</asp:Content>