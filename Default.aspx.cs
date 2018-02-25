using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PBF;

namespace Demo
{
    public partial class Default : System.Web.UI.Page
    {
        public struct Keypair
        {
            public string publicKey;
            public string privateKey;
        }

        private Keypair currKey = new Keypair
        {
            publicKey = "MIGdMA0GCSqGSIb3DQEBAQUAA4GLADCBhwKBgQCiH/Xn9IDnV7d8gwquMUahZ5NSPeOCSynOoXEkiXd5PE2dYTaB2tqrdk702QljcfhovAz7Vv8UOAcly+9yRrJKRJcnaJfdkLufiFG82v4oav6N6DK2umpSjonveM43N91NLyD4FislRAVEaMxicpuQj3Cr12VK1nVy62KpdpFB+wIBAw==",
            privateKey = "MIICpDAeBgoqhkiG9w0BDAEDMBAECgECAwQFBgcICQoCAgPoBIICgCULKs/w+nw7JHnSD74U7x0ju7+L7icP0ZfWdMKfT0/N7feJCgPibgXO6NCFCpzCIjHWFf5P8cHAYwm9C4EAyJJfEuQ3GOF6nGGjI3NNn1/nW25rl9IWSep6b7HoFGt2oGLZra/dEeC6HDjOGp9cGHl/CL2X5/O22Nd58ewmEaktGr3/l5NeZ+DWqAIMRiZRGUs3zpSSvyNFNJdadIWBmeSYOSutcsLc4YhuX2qNFrpi0svQm+c+OAtzwWFrt/hcQWLU2tRYHNI6leVNWOW+Y0KYIWJQt9ld1dsfuKbrPicHB1pHI6ikx7UtRi12Y6BtkBTB2wDEYjFBlEfXkSNc64S0tyNbRrEhS/u+CYgobbXe3HF5+aKixhj8dHMxl2acEVunwu0sc0Aj2k/9v4UXaui58LQlULqzZT2y0DehmKy5yw/HNXTaQyCWvbOx9lNTd7kLv63WLwgnvuhuRr9cX+g+LKi8Fbj2MMKigpLFXVIF/qO1oukyKG31KOyuxBXE+dRue8Apuk8zJIZ7N58TJOqOsKKnrb+qa8VdYvZrUEtJqw0sjsS59lxL6pxi+7TL9n7/oH+4sZnclj/eliJ9Ifrnc4RkGpHxVVHMca8lTtrpe6zYJeAoubWgRtRkjo5nOfxEmsSNJrpCSMh3/04jwJWTHGBCe1H7OIUMJVDbWOCBHXPytbZtNEsqtvR9q7z7Jc+4Tcn4Sgidmq9S67Dq9GMLEG1ZBBJZosil1hcZ2EPGcWMU3TAsoLnkaLtAFOLaiVbPY0kVRKi1BPcgNsbzydhbew6O/AN00XBycuo4gZWL+NDkPlpUrLpyHfKt92AJmuImZ4TKbQbtlDFGv+xaNM4="
        };

        private string password = "123456";

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                currPublicKey.Text = currKey.publicKey;
                currPrivateKey.Text = currKey.privateKey;
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEncrypt_Click(object sender, EventArgs e)
        {
            string s=txtString.Text;
            string newString="";
            if (!PBF.KeypairHelper.EncryptByPublicKey(s, currKey.publicKey, ref newString))
                Response.Write(PBF.KeypairHelper.Error);
            else
                txtString.Text = newString;
        }

        protected void btnDecrypt_Click(object sender, EventArgs e)
        {
            string s = txtString.Text;
            string newString = "";

            if (!PBF.KeypairHelper.DecryptByPrivateKey(s, currKey.privateKey, password, ref newString))
                Response.Write(PBF.KeypairHelper.Error);
            else
                txtString.Text = newString;
        }

        protected void btnNewKey_Click(object sender, EventArgs e)
        {
            string mykey = myKey.Text;

            KeypairHelper.KeyPair newKeypair=new KeypairHelper.KeyPair();

            if(!PBF.KeypairHelper.CreateRSAKeypair(mykey, ref newKeypair))
                Response.Write(PBF.KeypairHelper.Error);
            else
            {
                newPublicKey.Text = newKeypair.publicKey;
                newPrivateKey.Text = newKeypair.privateKey;
            }
        }


    }
}