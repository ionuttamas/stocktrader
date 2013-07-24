<%@ Page Buffer="true" Title="Glossary" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="Trade.Web.Glossary" Codebehind="Glossary.aspx.cs" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    
    <h2>
       Glossary
    </h2>
    <br />
    <table class="TermsTableStyle" align="center">
     <tr>
     <th class="TradeConfigTHStyle">Term</th>
     <th class="TradeConfigTHStyle">Description</th>
     </tr>
     <tr>
     <td class="TradeTermsTDStyleLeft"> Account ID</td><td class="TradeTermsTDStyleRight">A unique Integer based key. Each user is assigned an account ID at account creation time.</td>
     </tr>
     <tr>
     <td class="TradeTermsTDStyleLeft"> Account Created</td><td class="TradeTermsTDStyleRight">The time and date the users account was first created.</td>
     </tr>
     <tr>
     <td class="TradeTermsTDStyleLeft"> Cash Balance</td><td class="TradeTermsTDStyleRight"> The current cash balance in the users account. This does not include current stock holdings.</td>
     </tr>
     <tr>
     <td class="TradeTermsTDStyleLeft"> Company</td><td class="TradeTermsTDStyleRight">The full company name for an individual stock.</td>
     </tr>
     <tr>
     <td class="TradeTermsTDStyleLeft"> Current Gain/Loss</td><td class="TradeTermsTDStyleRight">The total gain or loss of this account, computed by substracting the current sum of cash/holdings minus the opening account balance.</td>
     </tr>
     <tr>
     <td class="TradeTermsTDStyleLeft"> Current Price</td><td class="TradeTermsTDStyleRight">The current trading price for a given stock symbol.</td>
     </tr>
     <tr>
     <td class="TradeTermsTDStyleLeft"> Gain/Loss</td><td class="TradeTermsTDStyleRight">The current gain or loss of an individual stock holding, computed as (current market value - holding basis).</td>
     </tr>
     <tr>
     <td class="TradeTermsTDStyleLeft"> Last Login</td><td class="TradeTermsTDStyleRight">The date and time this user last logged in to Trade.</td>
     </tr>
     <tr>
     <td class="TradeTermsTDStyleLeft"> Market Value</td><td class="TradeTermsTDStyleRight">The current total value of a stock holding, computed as (quantity * current price).</td>
     </tr>
     <tr>
     <td class="TradeTermsTDStyleLeft"> Number of Holdings</td><td class="TradeTermsTDStyleRight">The total number of stocks currently owned by this account.</td>
     </tr>
     <tr>
     <td class="TradeTermsTDStyleLeft"> Open Price</td><td class="TradeTermsTDStyleRight">The price of a given stock at the open of the trading session.</td>
     </tr>
     <tr>
     <td class="TradeTermsTDStyleLeft"> Order Id</td><td class="TradeTermsTDStyleRight">A unique Integer based key. Each order is assigned an order ID at order creation time.</td>
     </tr>
     <tr>
     <td class="TradeTermsTDStyleLeft"> Opening Balance</td><td class="TradeTermsTDStyleRight">The initial cash balance in this account when it was opened.</td>
     </tr>
     <tr>
     <td class="TradeTermsTDStyleLeft"> Order Status</td><td class="TradeTermsTDStyleRight">orders are opened, processed, closed and completed. Order status shows the current stat for this order.</td>
     </tr>
     <tr>
     <td class="TradeTermsTDStyleLeft"> Price Range</td><td class="TradeTermsTDStyleRight">The low and high prices for this stock during the current trading session</td>
     </tr>
     <tr>
     <td class="TradeTermsTDStyleLeft"> Purchase Date</td><td class="TradeTermsTDStyleRight">The date and time the a stock was purchased.</td>
     </tr>
     <tr>
    <td class="TradeTermsTDStyleLeft"> Purchase Price</td><td class="TradeTermsTDStyleRight">The price used when purchasing the stock.</td>
    </tr>
    <tr>
    <td class="TradeTermsTDStyleLeft"> Purchase Basis</td><td class="TradeTermsTDStyleRight">The total cost to purchase this holding. This is computed as (quantity * purchase price).</td>
     </tr>
    <tr>
    <td class="TradeTermsTDStyleLeft"> Quantity</td><td class="TradeTermsTDStyleRight">The number of stock shares in the order or user holding.</td>
    </tr>
    <tr>
    <td class="TradeTermsTDStyleLeft"> Session Created</td><td class="TradeTermsTDStyleRight">An HTTP session is created for each user at during login. Session created shows the time and day when the session was created.</td>
    </tr>
    <tr>
    <td class="TradeTermsTDStyleLeft"> Sum of Cash/Holdings</td><td class="TradeTermsTDStyleRight">The total current value of this account. This is the sum of the cash balance along with the value of current stock holdings.</td>
     </tr>
    <tr>
    <td class="TradeTermsTDStyleLeft"> Symbol</td><td class="TradeTermsTDStyleRight">The symbol for a Trade stock.</td>
     </tr>
    <tr>
    <td class="TradeTermsTDStyleLeft"> Total Logins</td><td class="TradeTermsTDStyleRight">The total number of logins performed by this user.</td>
     </tr>
    <tr>
    <td class="TradeTermsTDStyleLeft"> Total Logouts</td><td class="TradeTermsTDStyleRight">The total number of logouts performed by this user.</td>
    </tr>
    <tr>
    <td class="TradeTermsTDStyleLeft"> Total of Holdings</td><td class="TradeTermsTDStyleRight">The current total value of all stock holdings in this account given the current valuation of each stock held.</td>
    </tr>
    <tr>
    <td class="TradeTermsTDStyleLeft"> Top Gainers</td><td class="TradeTermsTDStyleRight">The list of stocks (matching LIKE CLAUSE 's:1__%') gaining the most in price during the current trading session.</td>
    </tr>
    <tr>
    <td class="TradeTermsTDStyleLeft"> Top Losers</td><td class="TradeTermsTDStyleRight">The list of stocks (matching LIKE CLAUSE 's:1__%')  falling the most in price during the current trading session.</td>
    </tr>
    <tr>
    <td class="TradeTermsTDStyleLeft"> Trade Stock Index (TSIA)</td><td class="TradeTermsTDStyleRight">A computed index of the top 20 stocks (matching LIKE CLAUSE 's:1__%') in Trade.</td>
    </tr>
    <tr>
    <td class="TradeTermsTDStyleLeft"> Trading Volume</td><td class="TradeTermsTDStyleRight">The total number of shares traded for stocks (matching LIKE CLAUSE 's:1__%') during this trading session.</td>
    </tr>
    <tr>
    <td class="TradeTermsTDStyleLeft"> Txn Fee</td><td class="TradeTermsTDStyleRight">The fee charged by the brokerage to process this order.</td>
    </tr>
    <tr>
    <td class="TradeTermsTDStyleLeft"> Type</td><td class="TradeTermsTDStyleRight">The order type (buy or sell).</td>
    </tr>
    <tr>
    <td class="TradeTermsTDStyleLeft"> User ID</td><td class="TradeTermsTDStyleRight">The unique user ID for the account chosen by the user at account registration.</td>
    </tr>
    <tr>
    <td class="TradeTermsTDStyleLeft"> Volume</td><td class="TradeTermsTDStyleRight">The total number of shares traded for this stock.</td>
    </tr>
</table>
</center>
<br />
</td>
</tr>
</table>
    
</asp:Content>

