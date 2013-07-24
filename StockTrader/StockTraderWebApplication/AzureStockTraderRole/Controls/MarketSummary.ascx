<%@Control Language="C#"  AutoEventWireup="false" Inherits="Trade.Web.MarketSummary" Codebehind="MarketSummary.ascx.cs" %>
<%@Import Namespace = "Trade.StockTraderWebApplicationSettings" %>
<!-- Start MarketSummary ASP.NET User Control -->
<table class="MarketSummaryControlTableStyle" align="center">
    <tr >
       <td colspan="2" style="font-size:1.1em;font-weight:bold;text-align:center">Market Summary<div style="font-size:0.8em;font-weight:normal">
       <asp:Label ID="summaryDate" runat="server" Text="summaryDate"></asp:Label></div></td>
    </tr>
    <tr >
       <th class="MarketSummaryHeader" style="background-image:url('Images/MktSummaryLeftFill.png');"><a href="/Glossary.aspx">Trade Stock<br />Index (TSIA)</a></th>
       <td class="MarketSummaryLeftSubHeader" style="background-image:url('Images/MktSummaryRightFill.png');">
       <asp:Label ID="TSIA" runat="server" Text="TSIA"></asp:Label>
       <asp:Label ID="GainPercent" runat="server" Text="Gain"></asp:Label></td>
    </tr>
    <tr >
       <th class="MarketSummaryHeader" style="background-image:url('Images/MktSummaryLeftFill.png');"><a href="/Glossary.aspx"  >Trading<br />Volume</a>
       </th>
       <td class="MarketSummaryLeftSubHeader" style="background-image:url('Images/MktSummaryRightFill.png');">
       <asp:Label ID="Volume" runat="server" Text="Volume"></asp:Label></td>
       </tr>
    <tr >
    <th class="MarketSummaryHeader" style="background-image:url('Images/MktSummaryLeftFill.png');"><a href="/Glossary.aspx"  >Top Gainers</a></th>
       <td class="MarketSummaryGainerLoserHeader" style="padding-bottom:10px;background-image:url('Images/MktSummaryRightFill.png');">
<!-- Begin Repeater for Top Gainers -->
       <asp:Repeater id="TopGainers" runat="server">
           <HeaderTemplate>
              <table class="MarketSummaryControlGainersLosersTableStyle">
                 <tr>
                     <th class="MarketSummaryInnerHeading">Symbol</th> <th class="MarketSummaryInnerHeading">Price</th> <th class="MarketSummaryInnerHeading">Change</th>
                 </tr>
          </HeaderTemplate>
          <ItemTemplate>
                    <tr>
                        <td class="MarketSummaryInnerData"><%# Eval("quoteLink")%></td>
                        <td class="MarketSummaryInnerData" style="text-align:right"><%# Eval("priceWithArrow")%></td>
                        <td class="MarketSummaryInnerData" style="text-align:right"><%# Eval("gainWithArrow")%></td>
                    </tr>
          </ItemTemplate>
          <FooterTemplate>
            </table>
          </FooterTemplate>
       </asp:Repeater>
<!-- End Repeater for Top Gainers -->
       </td>
   </tr>
   <tr >
       <th class="MarketSummaryHeader"  style="background-image:url('Images/MktSummaryLeftFill.png');"> <a href="/Glossary.aspx"  >Top Losers</a></th>
       <td class="MarketSummaryGainerLoserHeader" style="padding-bottom:10px;background-image:url('Images/MktSummaryRightFill.png');">
<!-- Begin Repeater for Top Losers -->
       <asp:Repeater id="TopLosers" runat="server">
           <HeaderTemplate>
             <table class="MarketSummaryControlGainersLosersTableStyle">
                <tr>
                    <th class="MarketSummaryInnerHeading">Symbol</th> <th class="MarketSummaryInnerHeading">Price</th> <th class="MarketSummaryInnerHeading">Change</th>
                </tr>
           </HeaderTemplate>
           <ItemTemplate>
                <tr>
                   <td class="MarketSummaryInnerData"><%# Eval("quoteLink")%> </td>
                   <td class="MarketSummaryInnerData" style="text-align:right"><%# Eval("priceWithArrow")%></td>
                   <td class="MarketSummaryInnerData" style="text-align:right"><%# Eval("gainWithArrow")%></td>
                </tr>
           </ItemTemplate>
           <FooterTemplate>
             </table>
           </FooterTemplate>
        </asp:Repeater>
<!-- End Repeater for Top Losers -->
        </td>
    </tr>  
</table>
<!-- End MarketSummary ASP.NET User Control -->