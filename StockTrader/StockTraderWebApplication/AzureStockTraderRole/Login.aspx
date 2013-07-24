<%@ Page Buffer="true" Title="Login" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="Trade.Web.Login" Codebehind="Login.aspx.cs" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    
    <h2>
        Login
    </h2>
   <br />
     
    <div style="text-align:center;">
             <asp:Image ID="LoginImage" runat="server" ImageUrl="~/Images/stock_market.jpg" 
            BorderStyle="Ridge" CssClass="center" ImageAlign="Middle" />
    </div>
   
      <table width="300px" align="center">
                <tr>
                        <td colspan="2">
                        <div class="Invalid">
                        <asp:Label runat="server" ID="InValid" CssClass="InvalidLogin" Text=""></asp:Label><br />
                        </div>
                        </td>
               </tr>
               <tr>
                        <td style="text-align:right;">User Name:</td>
                        <td style="text-align:left;padding-left:5px">
                        <asp:TextBox ID="uid" runat="server" Width="120" MaxLength="50" CausesValidation="True" 
                                CssClass="textEntry" ValidationGroup="Login" TabIndex="1"></asp:TextBox></td>
               </tr>
               <tr>
                        <td style="text-align:right;">Password:</td>
                        <td style="text-align:left;padding-left:5px">
                        <asp:TextBox ID="txtpassword" runat="server" Width="120" MaxLength="20" TextMode="Password" 
                                CssClass="textEntry" TabIndex="2"></asp:TextBox></td>                
               </tr>
                <tr>
                   <td></td>
                   <td style="text-align:left;padding-left:5px">
                  <p>
                  <br />
                    <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Log In" 
                          ValidationGroup="LoginUserValidationGroup" Width="75px" TabIndex="3" />
                   </p>
                   </td>
                </tr>
              </table>
              <center>
              <span style="font-size:.75em;">
              You can register, or login with 'uid-n'/'xxx' n=0-999
              </span>
             
              </center>
              <div class="Invalid">
              <br />
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
                                ValidationGroup="Login" />
                        <br />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorUserID" runat="server" ControlToValidate="uid"
                                Display="None" ErrorMessage="Please enter a user name!" ValidationGroup="Login"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidatorUserID" 
                                runat="server" ControlToValidate="uid" Display="None" 
                                ErrorMessage="User ID Must Be 3-50 Alphanumeric Characters" 
                                ValidationExpression="^[-A-Za-z0-9\\s]{3,50}$" ValidationGroup="Login"></asp:RegularExpressionValidator>
                        <br />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorPassword" runat="server" ControlToValidate="txtpassword"
                                Display="None" ErrorMessage="Please enter a password!" 
                                ValidationGroup="Login"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidatorPWD" 
                                runat="server" ControlToValidate="txtpassword" Display="None" 
                                ErrorMessage="Password Must Be 3-20 AlphaNumeric Characters, No Special Characters." 
                                ValidationExpression="^([a-zA-Z0-9]{3,20})$" ValidationGroup="Login"></asp:RegularExpressionValidator>
                                </div>
                <center>
                <p>
                <span style="font-size:1.1em;font-weight:bold;">First time user? &nbsp;<a href="register.aspx">Please Register!</a></span>
                </p>
                </center>
                <br />
</asp:Content>

