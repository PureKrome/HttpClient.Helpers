using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WorldDomination.Net.Http
{
    public class HttpContentEqualityComparer : IEqualityComparer<HttpContent>
    {
        public bool Equals(HttpContent x,
                           HttpContent y)
        {
            if (x == null &&
                y == null)
                return true;

            if (x == null ^ y == null)
                return false;

            if (!ContentHeadersAreEquivalent(x.Headers, y.Headers))
                return false;

            var sourceContent = x.ReadAsByteArrayAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var destinationContent = y.ReadAsByteArrayAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            return sourceContent.SequenceEqual(destinationContent);

        }

        private static bool ContentHeadersAreEquivalent(HttpContentHeaders source,
                                                        HttpContentHeaders destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (source.Count() != destination.Count())
            {
                return false;
            }

            foreach (var header in source)
            {
                if (!destination.Contains(header.Key))
                {
                    return false;
                }

                var sourceValues = source.GetValues(header.Key);
                var destinationValues = source.GetValues(header.Key);

                if (sourceValues.Count() != destinationValues.Count())
                {
                    return false;
                }

                foreach (var value in sourceValues)
                {
                    if (!destinationValues.Contains(value))
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        public int GetHashCode(HttpContent obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var hashCode = obj.GetType().FullName.GetHashCode();

            var content = obj.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            hashCode = hashCode ^ content.GetHashCode();

            if (obj.Headers != null)
            {
                foreach (var header in obj.Headers)
                {
                    var headerHash = header.Value.Aggregate(header.Key.GetHashCode(),
                                                            (current,
                                                             next) => current ^ next.GetHashCode());

                    hashCode = hashCode ^ headerHash;
                }
            }

            return hashCode;
        }
    }
}