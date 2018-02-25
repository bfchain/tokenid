<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Demo.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        当前密钥对：<br />
        公钥：<asp:TextBox ID="currPublicKey" runat="server" Height="48px" ReadOnly="True" Width="672px"></asp:TextBox>
        <br />
        <br />
        私钥：<asp:TextBox ID="currPrivateKey" runat="server" Height="44px" ReadOnly="True" Width="671px"></asp:TextBox>
        <br />
        <br />
        请输入待加/解密字符串：<asp:TextBox ID="txtString" runat="server" Height="60px" Width="537px"></asp:TextBox>
        <asp:Button ID="btnEncrypt" runat="server" OnClick="btnEncrypt_Click" Text="加密" />
        <asp:Button ID="btnDecrypt" runat="server" OnClick="btnDecrypt_Click" Text="解密" />
        <br />
        <br />
        <br />
        <br />
        <br />
        <asp:Button ID="btnNewKey" runat="server" OnClick="btnNewKey_Click" Text="生成新的密钥对：" />
        <br />
        <br />
        密钥：<asp:TextBox ID="myKey" runat="server" MaxLength="8"></asp:TextBox>
        8位字符串<br />
        <br />
        公钥：<asp:TextBox ID="newPublicKey" runat="server" Height="66px" Width="683px"></asp:TextBox>
        <br />
        <br />
        私钥：<asp:TextBox ID="newPrivateKey" runat="server" Height="69px" Width="679px"></asp:TextBox>
    
    </div>
    </form>
</body>
</html>
