<%@ Page Buffer="true" Title="StockTrader Register" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="Trade.Web.Register" Codebehind="Register.aspx.cs" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    
    <h2>
        Register
    </h2>
    <table class="tradetable" align="center">
    <tr >
            <td colspan="4">
            <div class="action">
            Create Account Profile
            <br />
            </div>
             <div class="Invalid">
             <asp:Label ID="RegisterMessage" runat="server" Text=""></asp:Label>
            <br />
            </div>
            </td>
    </tr>
    <tr>
            <td style="text-align:right; padding-right:4px;padding-top:10px">Requested User ID:</td>
            <td style="text-align:left;padding-left:4px; padding-top:10px;"><asp:TextBox ID="UserID" CssClass="textEntry" runat="server" MaxLength="50" TabIndex="1"></asp:TextBox></td>
            <td style="text-align:right; padding-right:4px; padding-top:10px">Opening Balance:</td>
            <td style="text-align:left;padding-left:4px; padding-top:10px;">
                <asp:TextBox ID="OpenBalance" CssClass="textEntry" runat="server" Text="100000" 
                    MaxLength="7" TabIndex="5"></asp:TextBox></td>
    </tr>
    <tr>
            <td style="text-align:right; padding-right:4px;">Full Name:</td>
            <td style="text-align:left;padding-left:4px;"><asp:TextBox ID="FullName" 
                    CssClass="textEntry" runat="server" MaxLength="100" TabIndex="2"></asp:TextBox></td>
            <td style="text-align:right; padding-right:4px;">Email Address:</td>
            <td style="text-align:left;padding-left:4px;"><asp:TextBox ID="Email" 
                    CssClass="textEntry" runat="server" MaxLength="100" TabIndex="6"></asp:TextBox></td>
    </tr>
    <tr>
            <td style="text-align:right; padding-right:4px">Address:</td>
            <td style="text-align:left;padding-left:4px"><asp:TextBox ID="Address" 
                    CssClass="textEntry" runat="server" MaxLength="100" TabIndex="3"></asp:TextBox></td>
            <td style="text-align:right;padding-right:4px">Password:</td>
            <td style="text-align:left;padding-left:4px"><asp:TextBox ID="Password" 
                    CssClass="textEntry" runat="server" TextMode="Password" MaxLength="20" 
                    Width="150px" TabIndex="7"></asp:TextBox></td>
    </tr>
    <tr>
            <td style="text-align:right;padding-right:4px">Credit Card:</td>
            <td style="text-align:left;padding-left:4px"><asp:TextBox ID="CreditCard" 
                    CssClass="textEntry" runat="server" MaxLength="100" TabIndex="4" ></asp:TextBox></td>
            <td style="text-align:right;padding-right:4px">Confirm Password:</td>
            <td style="text-align:left;padding-left:4px"><asp:TextBox ID="ConfirmPassword" 
                    CssClass="textEntry" runat="server" TextMode="Password" MaxLength="20" 
                    Width="150px" TabIndex="8"></asp:TextBox></td>
    </tr>
    <tr>
             <td colspan="4" style="text-align:center;">
                    <p>
                    <br />
                    <asp:Button ID="RegisterButton" runat="server" CommandName="Register" 
                            Text="Register" ValidationGroup="Register" TabIndex="9"/>
                   </p>
             </td>
    </tr>
    <tr>
    <td colspan="4" style="text-align:center">
             <div class="Invalid">
              <br />
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Register" DisplayMode="List" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="FullName"
                    ErrorMessage="Please Enter a Name" ValidationGroup="Register" Display="None"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Address"
                    ErrorMessage="Please Enter an Address" ValidationGroup="Register" Display="None"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="Email"
                    ErrorMessage="Please Enter an Email" ValidationGroup="Register" Display="None"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="Password"
                    ErrorMessage="Please Enter a Password" ValidationGroup="Register" Display="None"></asp:RequiredFieldValidator>&nbsp;
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="Please Enter a User ID" ControlToValidate="UserID" Display="None" ValidationGroup="Register"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="Please Enter a Credit Card"
                    ValidationGroup="Register" ControlToValidate="CreditCard" Display="None"></asp:RequiredFieldValidator>
                <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="OpenBalance"
                    ErrorMessage="Enter a value between $1,000 and $10,000,000" ValidationGroup="Register" Display="None" MaximumValue="10000000" MinimumValue="1000"></asp:RangeValidator>
                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="Password"
                    ControlToValidate="ConfirmPassword" Display="None" ErrorMessage="Passwords Don't Match!"
                    ValidationGroup="Register"></asp:CompareValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="ConfirmPassword"
                    Display="None" ErrorMessage="Please Confirm Your Password" ValidationGroup="Register"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidatorName" 
                    runat="server" ControlToValidate="FullName" Display="None" 
                    ErrorMessage="The Full Name is not valid, note this release is extra sensitive to special characters as we are using regular expressions to validate all input." 
                    ValidationExpression="^[a-zA-Z0-9'\-\' '\.\,\'\\s]{1,60}$" 
                    ValidationGroup="Register"></asp:RegularExpressionValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidatorUserID" 
                    runat="server" ControlToValidate="UserID" Display="None" 
                    ErrorMessage="User ID not Valid" 
                    ValidationExpression="^[-A-Za-z0-9\\s]{3,50}$" ValidationGroup="Register"></asp:RegularExpressionValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidatorPassword" 
                    runat="server" ControlToValidate="Password" Display="None" 
                    ErrorMessage="Password must be between 3 and 20 characters, and must not contain special characters" 
                    ValidationExpression="^([a-zA-Z0-9]{3,20})$" ValidationGroup="Register"></asp:RegularExpressionValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidatorEmail" 
                    runat="server" ControlToValidate="Email" Display="None" 
                    ErrorMessage="Email not Valid" 
                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                    ValidationGroup="Register"></asp:RegularExpressionValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidatorCredit" 
                    runat="server" ControlToValidate="CreditCard" Display="None" 
                    ErrorMessage="Credit Card Not Valid" 
                    ValidationExpression="^[0-9-\s]{1,20}$" 
                    ValidationGroup="Register"></asp:RegularExpressionValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidatorAddress" 
                    runat="server" ControlToValidate="Address" Display="None" 
                    ErrorMessage="Address Not Valid!" 
                    ValidationExpression="^[a-zA-Z0-9'\-\,\#\.\;\' '\\s]{1,150}$" 
                    ValidationGroup="Register"></asp:RegularExpressionValidator>
                    </div>
            </td>
    </tr>
  </table>
    
</asp:Content>

