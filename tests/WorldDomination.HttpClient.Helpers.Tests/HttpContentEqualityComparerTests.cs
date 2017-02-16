using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using WorldDomination.Net.Http;
using Xunit;

namespace WorldDomination.HttpClient.Helpers.Tests
{
    public class HttpContentEqualityComparerTests
    {
        public static IEnumerable<HttpContent[]> EqualHttpContents
        {
            get
            {
                yield return new HttpContent[]
                {
                    new StringContent("{\"id\":1}", Encoding.UTF8),
                    new StringContent("{\"id\":1}", Encoding.UTF8)
                };

                yield return new HttpContent[]
                {
                    new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("a", "b"),
                        new KeyValuePair<string, string>("c", "1")
                    }),
                    new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("a", "b"),
                        new KeyValuePair<string, string>("c", "1")
                    })
                };

                yield return new HttpContent[]
                {
                    new ByteArrayContent(new byte[] {1,2,3,4,5,6}),
                    new ByteArrayContent(new byte[] {1,2,3,4,5,6})
                };
            }
        }

        public static IEnumerable<HttpContent[]> EquivalentHttpContents
        {
            get
            {
                yield return new HttpContent[]
                {
                    new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("a", "b"),
                        new KeyValuePair<string, string>("c", "1")
                    }),
                    new StringContent("a=b&c=1", Encoding.UTF8, "application/x-www-form-urlencoded")
                };
            }
        }

        [Theory]
        [MemberData(nameof(EqualHttpContents))]
        public void WhenTwoEqualItems_Equals_ShouldReturnTrue(HttpContent first, HttpContent second)
        {
            var comparer = new HttpContentEqualityComparer();

            var result = comparer.Equals(first, second);

            result.ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(EquivalentHttpContents))]
        public void WhenTwoEquivalentItems_Equals_ShouldReturnTrue(HttpContent first, HttpContent second)
        {
            var comparer = new HttpContentEqualityComparer();

            var result = comparer.Equals(first, second);

            result.ShouldBeTrue();
        }
    }
}
