<%@ Page Buffer="true" Title="Stock Trade" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="Trade.Web.StockTrade" Codebehind="StockTrade.aspx.cs" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Trade Confirmation
    </h2>
    <br />
    <asp:Panel ID="PanelConfirm" runat="server">
          <table class="StockTradeTableStyle" align="center" style="background-image:url('Images/OrderFill.png');">
              <tr>
                 <td colspan="4" style="font-size:12px;padding-top:8px;">
                 <asp:Label ID="TradeOperation" runat="server"></asp:Label>
                </td>
              </tr>
              <tr>
                <td style="text-align:right;padding-left:4px; padding-right:4px;">
                Number of Shares:
                </td>
                <td style="text-align:left;padding-left:4px;">
                <asp:TextBox ID="quantity" runat="server" MaxLength="7" Width="80px"  CssClass="textEntry">100</asp:TextBox>
                    <asp:TextBox ID="TextBoxID" runat="server" Visible="False"></asp:TextBox>
                </td>
                <td  style="padding-top:10px;text-align:right;padding-bottom:12px;">
                    <asp:Button ID="ButtonTrade" runat="server" Text="Buy" 
                        onclick="ButtonTrade_Click" Width="70px" />
                </td>
                <td  style="padding-top:10px; padding-bottom:12px">
                    <asp:Button ID="ButtonCancel" runat="server" Text="Cancel" 
                        onclick="ButtonCancel_Click" Width="70px" />
                </td>
             </tr>
          </table>    
          <center>
                <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="quantity"
                    ErrorMessage="Please enter a valid value..." 
              MaximumValue="100000000" MinimumValue="1"
                    Type="Integer" ForeColor="Red"></asp:RangeValidator><br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                    ControlToValidate="quantity" ErrorMessage="Please enter a value" 
                    ForeColor="Red"></asp:RequiredFieldValidator>
                <br />
                     <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                         ControlToValidate="TextBoxID" ErrorMessage="Illegal Input Found!" 
                         ValidationExpression="^[A-Za-z0-9'':'';'','\\s]{3,20}$" 
              ForeColor="Red"></asp:RegularExpressionValidator>
                    <br />
                     </center>
                     </asp:Panel>
             <asp:Panel ID="PanelTrade" runat="server">
             <table class="OrderConfirmTableStyle" align="center">
             <tr >
                    <td colspan="8" class="OrderConfirmMessageStyle" style="padding-left:15px;"><asp:Label ID="ConfirmMessage" runat="server"></asp:Label></td>
             </tr>
             <tr>
                <th class="InnerHeading">
                Order ID</th>
                <th class="InnerHeading">
                Order Status</th>
                <th class="InnerHeading">
                Creation Date</th>
                <th class="InnerHeading">
                Submitted Date</th>
               <th class="InnerHeading">
                Txn Fee</th>
               <th class="InnerHeading">
                Type</th>
                <th class="InnerHeading">
                Symbol</th>
                <th class="InnerHeading">
                Quantity</th>
            </tr>       
            <tr>
                <td class="InnerData2">
                <asp:Label ID="OrderID" runat="server"></asp:Label></td>
                <td class="InnerData2" style="text-align:center">
                <asp:Label ID="OrderStatus" runat="server"></asp:Label></td>
                <td class="InnerData2" style="text-align:center">
                <asp:Label ID="OpenDate" runat="server"></asp:Label></td>
                <td class="InnerData2" style="text-align:center">
                <asp:Label ID="CompletionDate" runat="server"></asp:Label></td>
                <td class="InnerData2">
                <asp:Label ID="OrderFee" runat="server"></asp:Label></td>
                <td class="InnerData2" style="text-align:center">
                <asp:Label ID="OrderType" runat="server"></asp:Label></td>
                <td class="InnerData2" style="text-align:center">
                <asp:Label ID="Symbol" runat="server"></asp:Label></td>
                <td class="InnerData2">
                <asp:Label ID="QtyTraded" runat="server"></asp:Label></td>
           </tr>
       </table>
       </asp:Panel>   
       <br />
       <br />
<hr style="vertical-align:bottom"/>
 <table style="width:600" align="center">
    <tr>
        <td style="width:100px;height:30px;text-align:right">
            <asp:TextBox ID="symbols" CssClass="textEntry" runat="server" 
                Text="s:100;s:101;s:102;s:103" Width="150px" TabIndex="8"></asp:TextBox>
        </td>
        <td style="padding-left:10px;padding-bottom:4px;">
            <asp:Button ID="QuoteButton" runat="server"  CommandName="Quotes" 
                Text="Get Quotes" Width="100px" onclick="QuoteButton_Click" TabIndex="9" />
        </td>
    </tr>
</table>
<br />
</asp:Content> 