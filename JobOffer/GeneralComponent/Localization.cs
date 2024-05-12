using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MimeKit;
using MimeKit.Text;
using System;
using System.IO;

namespace JobOffer.GeneralComponent
{
    public class Localization
    {
        #region Method


        #region sendingEmail
        public void sendEmail(string recevierEmail, string subject, string body)
        {
            var emaila = new MimeMessage();
            emaila.From.Add(MailboxAddress.Parse("mlkmsbh84@outlook.com"));
            emaila.To.Add(MailboxAddress.Parse(recevierEmail));



            emaila.Subject = subject;
            emaila.Body = new TextPart(TextFormat.Text)
            {
                Text = body
            };


            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.outlook.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("mlkmsbh84@outlook.com", "1234mlok1234");
                smtp.Send(emaila);
                smtp.Disconnect(true);
            }
        }
        #endregion

        #region SaveImage

        public string SaveImage(IWebHostEnvironment webHostEnviroment, string fileName, string folderName, IFormFile copyFile)
        {
            
            if (!string.IsNullOrEmpty(fileName))
            {
                string fullFileName = Guid.NewGuid().ToString() + "_" + fileName;

                string path = Path.Combine(webHostEnviroment.WebRootPath + "/" + folderName + "/" + fullFileName);
                using (var filestream = new FileStream(path, FileMode.Create))
                {
                    copyFile.CopyToAsync(filestream);
                }
                return fullFileName;
            }


            else
            {
                return string.Empty;
            }

        }

        #endregion

        #endregion
    }
}
