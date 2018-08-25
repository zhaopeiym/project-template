using ProjectNameTemplate.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ProjectNameTemplate.Tests.Temp
{
    public class EncryptDecryptExtension_tests
    {
        [Fact]
        public void Test()
        {
            var key = "qwekdjfngmfkdjek";
            var encryptObj = "123".DES3Encrypt(key);
            var decryptOjb = encryptObj.DES3Decrypt(key);
            Assert.True(decryptOjb == "123");
        }
    }
}
