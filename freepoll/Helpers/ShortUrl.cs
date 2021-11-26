﻿using System;
using System.Linq;

namespace freepoll.Helpers
{
    public static class ShortUrl
    {
        public static string GenerateShortUrl()
        {
            string urlsafe = string.Empty;
            Enumerable.Range(48, 75)
              .Where(i => i < 58 || i > 64 && i < 91 || i > 96)
              .OrderBy(o => new Random().Next())
              .ToList()
              .ForEach(i => urlsafe += Convert.ToChar(i));
            int maxLength = new Random().Next(3, 6);
            int minLength = new Random().Next(0, urlsafe.Length - 6);
            string token = urlsafe.Substring(minLength, maxLength);

            return token;
        }
    }
}