using System;
using AlibabaCloud.SDK.VIAPI.Utils;

/// <summary>
/// Summary description for Class1
/// </summary>
public class Class1
{
	public Class1()
	{
        static void Main(string[] args)
        {
            string accessKeyId = "accessKeyId";   // 您的AccessKeyID
            string accessSecret = "accessKeySecret";   // 您的AccessKeySecret
            string imageUrl = "https://tse1-mm.cn.bing.net/th/id/R-C.c82590d4c1f8e1a8b8ffeb5c7674715f?rik=plgAnz%2bkRhPqnw&riu=http%3a%2f%2fdoctormacro.com%2fImages%2fBergman%2c+Ingrid%2fAnnex%2fAnnex+-+Bergman%2c+Ingrid+(Indiscreet)_01.jpg&ehk=G4XQuJisjx734aoE4B4zIn%2fqvwxePLIcpCSSgnwzw7s%3d&risl=&pid=ImgRaw&r=0";  // 上传成功后，返回上传后的文件地址
            FileUtils fileobj = FileUtils.getInstance(accessKeyId, accessSecret);
            string result = fileobj.Upload(imageUrl);
            Console.WriteLine(result);
        }
    }
}
