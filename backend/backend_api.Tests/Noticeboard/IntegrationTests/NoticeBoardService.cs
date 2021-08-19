﻿using System;
using System.Linq;
using System.Net;
using backend_api.Data.NoticeBoard;
using backend_api.Models.Enumerations;
using backend_api.Models.Forum.Responses;
using backend_api.Models.NoticeBoard;
using backend_api.Models.NoticeBoard.Requests;
using backend_api.Models.NoticeBoard.Responses;
using backend_api.Services.NoticeBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace backend_api.Tests.Noticeboard.IntegrationTests
{
    public class NoticeboardService
    {
        private readonly NoticeBoard _mockNoticeboard;
        private NoticeBoardContext _noticeboardContext;

        public NoticeboardService()
        {
            var serviceProvider = new ServiceCollection().AddEntityFrameworkNpgsql().BuildServiceProvider();
            var builder = new DbContextOptionsBuilder<NoticeBoardContext>();
            
            var env = Environment.GetEnvironmentVariable("CONN_STRING");
            builder.UseNpgsql(env)
                .UseInternalServiceProvider(serviceProvider);

            _noticeboardContext = new NoticeBoardContext(builder.Options);

            _mockNoticeboard = new NoticeBoard(
                "Thread Title",
                "Thread Content",
                3,
                "string.url",
                UserRoles.Administrator,
                1
            );

            
        }
        [Fact(DisplayName = "Should return created HTTP response")]
        public async void CreateNoticeboardThread()
        {
            var noticeboardRepo = new NoticeBoardRepository(_noticeboardContext);
            var noticeboardService = new NoticeBoardService(noticeboardRepo);

            var req = new AddNoticeBoardThreadRequest(_mockNoticeboard.ThreadTitle, _mockNoticeboard.ThreadContent,
                _mockNoticeboard.MinEmployeeLevel, _mockNoticeboard.ImageUrl, _mockNoticeboard.PermittedUserRoles,
                _mockNoticeboard.UserId);

            var resp = noticeboardRepo.AddNoticeBoardThread(req);
                
            Assert.Equal(resp.Result.Response, HttpStatusCode.Created);
        }

        [Fact(DisplayName = "Should return HTTP status code Bad Request on Empty Title")]
        public void CreateNoticeBoardThreadEmptyTitle()
        {
            var noticeboardRepo = new NoticeBoardRepository(_noticeboardContext);
            var noticeboardService = new NoticeBoardService(noticeboardRepo);

            var req = new AddNoticeBoardThreadRequest("", _mockNoticeboard.ThreadContent,
                _mockNoticeboard.MinEmployeeLevel, _mockNoticeboard.ImageUrl, _mockNoticeboard.PermittedUserRoles,
                _mockNoticeboard.UserId);

            var resp = noticeboardService.AddNoticeBoardThread(req);
            
            Assert.Equal(HttpStatusCode.BadRequest, resp.Result.Response);
        }
        
        [Fact(DisplayName = "Should return HTTP status BadRequest on Empty Thread Body")]
        public void CreateNoticeBoardThreadEmptyBody()
        {
            var noticeboardRepo = new NoticeBoardRepository(_noticeboardContext);
            var noticeboardService = new NoticeBoardService(noticeboardRepo);

            var req = new AddNoticeBoardThreadRequest(_mockNoticeboard.ThreadTitle, "",
                _mockNoticeboard.MinEmployeeLevel, _mockNoticeboard.ImageUrl, _mockNoticeboard.PermittedUserRoles,
                _mockNoticeboard.UserId);

            var resp = noticeboardService.AddNoticeBoardThread(req);
            
            Assert.Equal(HttpStatusCode.BadRequest, resp.Result.Response);
        }

        [Fact(DisplayName = "Should return HTTP status code Bad Request on Invalid user Id")]
        public void CreateNoticeBoardThreadNullUser()
        {
            var noticeboardRepo = new NoticeBoardRepository(_noticeboardContext);
            var noticeboardService = new NoticeBoardService(noticeboardRepo);

            var req = new AddNoticeBoardThreadRequest(_mockNoticeboard.ThreadTitle, _mockNoticeboard.ThreadContent,
                _mockNoticeboard.MinEmployeeLevel, _mockNoticeboard.ImageUrl, _mockNoticeboard.PermittedUserRoles,
                0);

            var resp = noticeboardService.AddNoticeBoardThread(req);
            
            Assert.Equal(HttpStatusCode.BadRequest, resp.Result.Response);
        }

        [Fact(DisplayName = "Should return Delete HTTP response")]
        public async void DeleteNoticeBoardThread()
        {
            var noticeboardRepo = new NoticeBoardRepository(_noticeboardContext);
            var noticeboardService = new NoticeBoardService(noticeboardRepo);

            var Addreq = new AddNoticeBoardThreadRequest(_mockNoticeboard.ThreadTitle, _mockNoticeboard.ThreadContent,
                _mockNoticeboard.MinEmployeeLevel, _mockNoticeboard.ImageUrl, _mockNoticeboard.PermittedUserRoles,
                _mockNoticeboard.UserId);

            var addResp = noticeboardRepo.AddNoticeBoardThread(Addreq);
            
            Assert.Equal(addResp.Result.Response, HttpStatusCode.Created);

            var req = new RetrieveNoticeBoardThreadsRequest();
            var noticeBoardThreads = await noticeboardRepo.RetrieveAllNoticeBoardThreads(req);
            
            Assert.NotEmpty(noticeBoardThreads);

            var deleteReq = new DeleteNoticeBoardThreadRequest(noticeBoardThreads.Last().ThreadId);
            var deleteResponse = noticeboardService.DeleteNoticeBoardThread(deleteReq);
            
            Assert.Equal(HttpStatusCode.Accepted, deleteResponse.Result.HttpStatusCode);

        }

        [Fact(DisplayName = "Should return accepted Http response")]
        public async void EditNoticeBoardThread()
        {
            var noticeboardRepo = new NoticeBoardRepository(_noticeboardContext);
            var noticeboardService = new NoticeBoardService(noticeboardRepo);
            
            var Addreq = new AddNoticeBoardThreadRequest(_mockNoticeboard.ThreadTitle, _mockNoticeboard.ThreadContent,
                _mockNoticeboard.MinEmployeeLevel, _mockNoticeboard.ImageUrl, _mockNoticeboard.PermittedUserRoles,
                _mockNoticeboard.UserId);

            var addResp = noticeboardRepo.AddNoticeBoardThread(Addreq);
            
            Assert.Equal(addResp.Result.Response, HttpStatusCode.Created);
            
            var req = new RetrieveNoticeBoardThreadsRequest();
            var noticeBoardThreads = await noticeboardRepo.RetrieveAllNoticeBoardThreads(req);

            var threadToEdit = noticeBoardThreads.Last();
            var editThreadRequest = new EditNoticeBoardThreadRequest(threadToEdit.ThreadId, threadToEdit.ThreadTitle,
                threadToEdit.ThreadContent, threadToEdit.MinEmployeeLevel, threadToEdit.ImageUrl,
                threadToEdit.PermittedUserRoles, threadToEdit.UserId);

            var editThreadResponse = noticeboardService.EditNoticeBoardThread(editThreadRequest);
            
            Assert.Equal(HttpStatusCode.Accepted, editThreadResponse.Result.Response);

        }
        
        
    }
}