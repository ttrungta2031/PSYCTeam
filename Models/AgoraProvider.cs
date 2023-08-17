using AgoraIO.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PsychologicalCounseling.Models
{
    public interface IAgoraProvider
    {
        string GenerateToken(string channel, string uId, uint expiredTime);
    }
    public class AgoraProvider : IAgoraProvider
    {
        public string GenerateToken(string channel, string uId, uint expiredTime)
        {
            try
            {
                var tokenBuilder = new AccessToken("249ed20e39a7470f9e7ed035b2fa4022", "d1811087e6ee4e3b930ab9c25d1c3ccd", channel, uId);

                tokenBuilder.addPrivilege(Privileges.kJoinChannel, expiredTime);

                tokenBuilder.addPrivilege(Privileges.kPublishAudioStream, expiredTime);

                tokenBuilder.addPrivilege(Privileges.kPublishVideoStream, expiredTime);

                tokenBuilder.addPrivilege(Privileges.kPublishDataStream, expiredTime);

                tokenBuilder.addPrivilege(Privileges.kRtmLogin, expiredTime);

                return tokenBuilder.build();
            } catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return "";
            }
        }
    }
}
