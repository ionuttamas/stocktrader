<%@ Register TagPrefix="controls" TagName="AccountOrders" Src = "Controls/AccountOrders.ascx"  %>
<%@ Register TagPrefix="controls" TagName="ClosedOrders" Src = "Controls/ClosedOrders.ascx"  %>
<%@ Page Buffer="true" Async="true" Title="Account Summary" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"  MaintainScrollPositionOnPostback="true" Inherits="Trade.Web.Account" Codebehind="Account.aspx.cs" %>
<%@Import Namespace="Trade.StockTraderWebApplicationModelClasses" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
   
    <h2>
        Account Summary
    </h2>
    <br />
     <controls:ClosedOrders id="ClosedOrdersControl" runat="server" ></controls:ClosedOrders>
    <table class="OrderAlertTableStyle" align="center" >
                <tr>
                    <td align="center">
                        <table style="border-collapse:collapse">
                        <tr>
                            <th class="InnerHeading3" style="text-align:center">Subtotal Buys</th>
                            <th class="InnerHeading3" style="text-align:center">Subtotal Sells</th>
                            <th class="InnerHeading3" style="text-align:center">Subtotal Fees</th>
                            <th class="InnerHeading3" style="text-align:center">Net Impact Cash Balance</th>
                        </tr>
                        <tr>
                            <td class="InnerData2" style="text-align:right"><%=string.Format("{0:C}", AccountOrdersControl.orderData.subtotalBuy)%></td>
                            <td class="InnerData2" style="text-align:right"><%=string.Format("{0:C}", AccountOrdersControl.orderData.subtotalSell)%></td>
                            <td class="InnerData2" style="text-align:right"><%=string.Format("{0:C}", AccountOrdersControl.orderData.txnFeesSubtotal)%></td>
                            <td class="InnerData2" style="text-align:right"><%=AccountOrdersControl.orderData.netImpactCashBalance%></td>
                        </tr>
                       </table>
                   </td>
                </tr>
            </table>


<table class="TradeHomeTableStyle" align="center">

<tr>
    <td class="OrderAlertControlStyle" align="center">
        <br />
        <asp:Label ID="InvalidInput" runat="server" ForeColor="#FF3300"></asp:Label>
        <br />
        <table class="OrderAlertTableStyle" align="center">
        <tr>
            <td colspan="2">
                
                <br />
                <asp:Label ID="WASLimit" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="AccountSubHeadStyle" style="text-align:left">Total Orders Shown:<b><asp:Label
            ID="NumOrdersShown" runat="server" Text="numOrders"></asp:Label></b>
            </td>
            
            <td class="AccountSubHeadStyle" style="text-align:right"><asp:LinkButton ID="orderLink"
                    runat="server">orderlink</asp:LinkButton>
            </td>
        </tr>
        </table>
    </td>
    </tr>
   <tr>
 <td align="center"> <controls:AccountOrders id="AccountOrdersControl" runat="server" ></controls:AccountOrders></td>
 </tr>

</table>

<table class="TradeHomeTableStyle" align="center">
<tr>
    <td>
    <br /><br />
    <center>
    <table class="AccountTableStyle" width="710px" align="center" style="background-image:url('Images/AccountFill.png');">
    <tr >
    <td colspan="4" style="height:20px;text-align:center;padding-top:5px"><b><asp:Label ID="UpdateMessage" runat="server" Text="Update Account Profile: " ForeColor="#003f5f"></asp:Label></b>
    <br />
    <hr style="vertical-align:top"/>
    </td>
    </tr>
    <tr>
    <td>
        <table style="width:710px">
            <tr>
                <td style="text-align:right">Full Name:</td>
                <td style="text-align:left"><asp:TextBox ID="FullName" runat="server" 
                        MaxLength="100" CssClass="textEntry" Width="240px" TabIndex="1"></asp:TextBox></td>
                 <td style="text-align:right">Email Address:</td>
                <td style="text-align:left">
                    <asp:TextBox ID="Email" runat="server" MaxLength="100" 
                        CssClass="textEntry" Width="240px" TabIndex="4"></asp:TextBox></td>
            </tr>
            <tr>
                <td style="text-align:right">Address:</td>
                <td style="text-align:left">
                    <asp:TextBox ID="Address" runat="server" 
                        MaxLength="100" CssClass="textEntry" Width="240px" TabIndex="2"></asp:TextBox></td>
                <td style="text-align:right">Password:</td>
                <td style="text-align:left"><asp:TextBox ID="Password" runat="server" 
                        TextMode="Password" Width="240px" MaxLength="100"  CssClass="textEntry" 
                        TabIndex="5"></asp:TextBox></td>
            </tr>
            <tr>
                <td style="text-align:right">Credit Card:</td>
                <td style="text-align:left">
                    <asp:TextBox ID="CreditCard" runat="server" 
                        MaxLength="100" CssClass="textEntry" Width="240px" TabIndex="3"></asp:TextBox></td>
                <td style="text-align:right">Confirm Password:</td>
                <td style="text-align:left">
                    <asp:TextBox ID="ConfirmPassword" runat="server" 
                        TextMode="Password" MaxLength="100" CssClass="textEntry" Width="240px" 
                        TabIndex="6"></asp:TextBox></td>
            </tr>
         </table>
     </td>
    </tr>
    <tr>
    <td colspan="4" align="center"><p>
        <asp:Button ID="UpdateButton" runat="server"  
            CommandName="Update" ValidationGroup="AccountEntry"
            Text="Update" Width="100px" onclick="UpdateButton_Click" TabIndex="7" /></p>
    <br />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
            ValidationGroup="AccountEntry" ForeColor="Red" />
        
    </td>
    </tr>
    <tr>
    <td>
     <br />
        <table style="width:710px; border-collapse:collapse;">
            <tr>
                <td class="AccountDataHeaderStyle1">
                <a href="Glossary.aspx">Account ID:</a></td>
                <td class="AccountDataLabelStyle1">
                <asp:Label ID="AccountID" runat="server"></asp:Label></td>
                <td class="AccountDataHeaderStyle2">
                <a href="Glossary.aspx">Account Created:</a></td>
                <td class="AccountDataLabelStyle2">
                <asp:Label ID="CreationDate" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="AccountDataHeaderStyle1">
                <a href="Glossary.aspx">User ID:</a></td>
                <td class="AccountDataLabelStyle1">
                <asp:Label id="Name" runat="server"></asp:Label></td>
                <td class="AccountDataHeaderStyle2">
                <a href="Glossary.aspx">Last Login:</a></td>
                <td class="AccountDataLabelStyle2">
                <asp:Label ID="LastLogin" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="AccountDataHeaderStyle1">
                <a href="Glossary.aspx">Opening Balance:</a></td>
                <td class="AccountDataLabelStyle1">
                <asp:Label ID="OpenBalance" runat="server"></asp:Label></td> 
                <td class="AccountDataHeaderStyle2">
                <a href="Glossary.aspx">Total Logins:</a></td>
                <td class="AccountDataLabelStyle2">
                <asp:Label ID="LoginCount" runat="server"></asp:Label></td>                            
            </tr>
            <tr>
                <td class="AccountDataHeaderStyle1">
                <a href="Glossary.aspx">Cash Balance:</a></td>
                <td class="AccountDataLabelStyle1">
                <asp:Label ID="Balance" runat="server"></asp:Label></td>
                <td class="AccountDataHeaderStyle2">
                <a href="Glossary.aspx">Total Logouts:</a></td>
                <td class="AccountDataLabelStyle2">
                <asp:Label id="TotalLogout" runat="server"></asp:Label></td>

            </tr>
            </table>
             <br />
            </td>
            </tr>
        </table>
        </center>
    </td>
    </tr>
    <tr>
    <td>
    <div class="Invalid">
        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ForeColor="Red" runat="server" ControlToValidate="Email"
            Display="None" ErrorMessage="Please Enter an Email" 
            ValidationGroup="AccountEntry"></asp:RequiredFieldValidator>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ForeColor="Red" runat="server" ControlToValidate="CreditCard"
            Display="None" ErrorMessage="Please Enter a Credit Card" ValidationGroup="AccountEntry"></asp:RequiredFieldValidator>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ForeColor="Red" runat="server" ControlToValidate="Address"
            Display="None" ErrorMessage="Please Enter an Address" ValidationGroup="AccountEntry"></asp:RequiredFieldValidator>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ForeColor="Red" runat="server" ControlToValidate="FullName"
            Display="None" ErrorMessage="Please Enter a Name" ValidationGroup="AccountEntry"></asp:RequiredFieldValidator>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ForeColor="Red" runat="server" ControlToValidate="Password"
            Display="None" ErrorMessage="Please Enter a Password" ValidationGroup="AccountEntry"></asp:RequiredFieldValidator>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" ForeColor="Red" runat="server" ControlToValidate="ConfirmPassword"
            Display="None" ErrorMessage="Please Confirm the Password" ValidationGroup="AccountEntry"></asp:RequiredFieldValidator>
        <asp:CompareValidator ID="CompareValidator1" ForeColor="Red" runat="server" ControlToCompare="Password"
            ControlToValidate="ConfirmPassword" Display="None" ErrorMessage="Passwords Do Not Match!"
            ValidationGroup="AccountEntry"></asp:CompareValidator>
        <asp:RegularExpressionValidator ID="RegularExpressionValidatorName" 
            runat="server" ForeColor="Red" ControlToValidate="FullName" Display="None" 
            ErrorMessage="Entered Name Not Valid!" 
            ValidationExpression="^[a-zA-Z0-9'\-\' '\.\,\'\\s]{1,60}$" 
            ValidationGroup="AccountEntry"></asp:RegularExpressionValidator>

        <asp:RegularExpressionValidator ID="RegularExpressionValidatorAddress" 
            runat="server" ForeColor="Red" ControlToValidate="Address" Display="None" 
            ErrorMessage="Address is Not Valid!" 
             ValidationExpression="^[a-zA-Z0-9'\-\,\#\.\;\' '\\s]{1,150}$" 
            ValidationGroup="AccountEntry"></asp:RegularExpressionValidator>
        <asp:RegularExpressionValidator ID="RegularExpressionValidatorCreditCard" 
            runat="server" ForeColor="Red" ControlToValidate="CreditCard" Display="None" 
            ErrorMessage="Credit Card Number Not Valid!" 
            ValidationExpression="^[0-9-\s]{1,20}$" 
            ValidationGroup="AccountEntry"></asp:RegularExpressionValidator>
        <asp:RegularExpressionValidator ID="RegularExpressionValidatorEmail" 
            runat="server" ForeColor="Red" ControlToValidate="Email" Display="None" 
            ErrorMessage="Email Address not Valid!" 
            ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
            ValidationGroup="AccountEntry"></asp:RegularExpressionValidator>
        <asp:RegularExpressionValidator ID="RegularExpressionValidatorPWD" 
            runat="server" ForeColor="Red" Display="None" 
            ErrorMessage="Password must be between 3 and 20 characters, and must not contain special characters" 
            ValidationExpression="^([a-zA-Z0-9]{3,20})$" 
            ValidationGroup="AccountEntry" ControlToValidate="Password"></asp:RegularExpressionValidator>
        <br />
        
        </div>
       
    </td>
    </tr>
    <tr>
<td colspan="4" align="center"> <hr style="vertical-align:bottom"/></td>
</tr>
    <tr>
 <td colspan="4" align="center" >

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
</td>
</tr>
</table>
    
</asp:Content>

