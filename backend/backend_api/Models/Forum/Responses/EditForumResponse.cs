﻿using System.Net;

namespace backend_api.Models.Forum.Responses
{
    public class EditForumResponse
    {
        private HttpStatusCode _response;

        public EditForumResponse(HttpStatusCode response)
        {
            _response = response;
        }

        public EditForumResponse()
        {
            
        }
        public HttpStatusCode Response
        {
            get => _response;
            set => _response = value;
        }
    }
}