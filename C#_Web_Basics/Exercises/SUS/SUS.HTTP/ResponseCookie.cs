﻿using System.Text;

namespace SUS.HTTP
{
    public class ResponseCookie : Cookie
    {
        public ResponseCookie(string data)
            : base(data) { }

        public ResponseCookie(string name, string value) 
            : base(name, value) { }

        public int MaxAge { get; set; }

        public bool HttpOnly { get; set; }

        public string Path { get; set; }

        public override string ToString()
        {
            var cookieBuilder = new StringBuilder();
            cookieBuilder.Append($"{this.Name}={this.Value}; Path={this.Path};");

            if (MaxAge != 0)
            {
                cookieBuilder.Append($" Max-Age={this.MaxAge};");
            }

            if (this.HttpOnly)
            {
                cookieBuilder.Append(" HttpOnly;");
            }

            return cookieBuilder.ToString();
        }
    }
}
