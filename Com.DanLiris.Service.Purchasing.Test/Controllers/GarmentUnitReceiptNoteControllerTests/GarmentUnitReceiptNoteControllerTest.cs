﻿using AutoMapper;
using Com.DanLiris.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.DanLiris.Service.Purchasing.Lib.Interfaces;
using Com.DanLiris.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.DanLiris.Service.Purchasing.Lib.ViewModels.GarmentUnitReceiptNoteViewModels;
using Com.DanLiris.Service.Purchasing.Lib.Services;
using Com.DanLiris.Service.Purchasing.Test.Helpers;
using Com.DanLiris.Service.Purchasing.WebApi.Controllers.v1.GarmentUnitReceiptNoteControllers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using Xunit;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Purchasing.Test.Controllers.GarmentUnitReceiptNoteControllerTests
{
    public class GarmentUnitReceiptNoteControllerTest
    {
        private GarmentUnitReceiptNoteViewModel ViewModel
        {
            get
            {
                return new GarmentUnitReceiptNoteViewModel
                {
                    UId = null,
                    Items = new List<GarmentUnitReceiptNoteItemViewModel>
                    {
                        new GarmentUnitReceiptNoteItemViewModel()
                    }
                };
            }
        }

        private GarmentUnitReceiptNote Model
        {
            get
            {
                return new GarmentUnitReceiptNote
                {
                    Items = new List<GarmentUnitReceiptNoteItem>
                    {
                        new GarmentUnitReceiptNoteItem()
                    }
                };
            }
        }

        private ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            return serviceProvider;
        }

        private GarmentUnitReceiptNoteController GetController(Mock<IGarmentUnitReceiptNoteFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();
            if (validateM != null)
            {
                servicePMock
                    .Setup(x => x.GetService(typeof(IValidateService)))
                    .Returns(validateM.Object);
            }

            var controller = new GarmentUnitReceiptNoteController(servicePMock.Object, mapper.Object, facadeM.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        User = user.Object
                    }
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";

            return controller;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public void Should_Success_Get_All_Data()
        {
            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();
            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(new ReadResponse<object>(new List<object>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitReceiptNoteViewModel>>(It.IsAny<List<GarmentUnitReceiptNote>>()))
                .Returns(new List<GarmentUnitReceiptNoteViewModel> { ViewModel });

            GarmentUnitReceiptNoteController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data()
        {
            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();
            var mockMapper = new Mock<IMapper>();
            GarmentUnitReceiptNoteController controller = new GarmentUnitReceiptNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_By_User()
        {
            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                 .Returns(new ReadResponse<object>(new List<object>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitReceiptNoteViewModel>>(It.IsAny<List<GarmentUnitReceiptNote>>()))
                .Returns(new List<GarmentUnitReceiptNoteViewModel> { ViewModel });

            GarmentUnitReceiptNoteController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetByUser();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_By_User_With_Filter()
        {
            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(new ReadResponse<object>(new List<object>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitReceiptNoteViewModel>>(It.IsAny<List<GarmentUnitReceiptNote>>()))
                .Returns(new List<GarmentUnitReceiptNoteViewModel> { ViewModel });

            GarmentUnitReceiptNoteController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetByUser(filter: "{ 'IsPosted': false }");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data_By_User()
        {
            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(new ReadResponse<object>(new List<object>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitReceiptNoteViewModel>>(It.IsAny<List<GarmentUnitReceiptNote>>()))
                .Returns(new List<GarmentUnitReceiptNoteViewModel> { ViewModel });

            GarmentUnitReceiptNoteController controller = new GarmentUnitReceiptNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetByUser();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(ViewModel);

            var mockMapper = new Mock<IMapper>();

            GarmentUnitReceiptNoteController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_PDF_By_Id()
        {
            var Model = this.Model;

            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(ViewModel);
            mockFacade.Setup(x => x.GeneratePdf(It.IsAny<GarmentUnitReceiptNoteViewModel>()))
                .Returns(new MemoryStream());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentUnitReceiptNoteViewModel>(It.IsAny<GarmentUnitReceiptNote>()))
                .Returns(ViewModel);

            GarmentUnitReceiptNoteController controller = GetController(mockFacade, null, mockMapper);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";

            var response = controller.Get(It.IsAny<int>());
            Assert.NotEqual(null, response.GetType().GetProperty("FileStream"));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns((GarmentUnitReceiptNoteViewModel)null);

            var mockMapper = new Mock<IMapper>();

            GarmentUnitReceiptNoteController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitReceiptNote>>(It.IsAny<List<GarmentUnitReceiptNoteViewModel>>()))
                .Returns(new List<GarmentUnitReceiptNote>());

            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitReceiptNoteViewModel>()))
                .Verifiable();

            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentUnitReceiptNote>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitReceiptNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();
            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var mockMapper = new Mock<IMapper>();
            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();

            var controller = new GarmentUnitReceiptNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = await controller.Post(new GarmentUnitReceiptNoteViewModel());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitReceiptNote>>(It.IsAny<List<GarmentUnitReceiptNoteViewModel>>()))
                .Returns(new List<GarmentUnitReceiptNote>());

            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitReceiptNoteViewModel>()))
                .Verifiable();

            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<GarmentUnitReceiptNote>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(It.IsAny<int>(), ViewModel);
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitReceiptNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();
            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(It.IsAny<int>(), ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Update_Data()
        {
            var mockMapper = new Mock<IMapper>();
            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();

            var controller = new GarmentUnitReceiptNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = await controller.Put(It.IsAny<int>(), new GarmentUnitReceiptNoteViewModel());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitReceiptNote>>(It.IsAny<List<GarmentUnitReceiptNoteViewModel>>()))
                .Returns(new List<GarmentUnitReceiptNote>());

            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitReceiptNoteViewModel>()))
                .Verifiable();

            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.DeleteData(It.IsAny<int>(), ViewModel);
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Delete_Data()
        {
            var controller = new GarmentUnitReceiptNoteController(GetServiceProvider().Object, null, null);

            var response = await controller.DeleteData(It.IsAny<int>(), It.IsAny<GarmentUnitReceiptNoteViewModel>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_For_UnitDO()
        {
            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();
            mockFacade.Setup(x => x.ReadForUnitDO(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<object>());

            var mockMapper = new Mock<IMapper>();

            GarmentUnitReceiptNoteController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetForUnitDO();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_For_UnitDO()
        {
            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();
            mockFacade.Setup(x => x.ReadForUnitDO(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception(""));

            var mockMapper = new Mock<IMapper>();

            GarmentUnitReceiptNoteController controller = new GarmentUnitReceiptNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetForUnitDO();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_For_UnitDOHeader()
        {
            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();
            mockFacade.Setup(x => x.ReadForUnitDOHeader(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<object>());

            var mockMapper = new Mock<IMapper>();

            GarmentUnitReceiptNoteController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetForUnitDOHeader();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_For_UnitDOHeader()
        {
            var mockFacade = new Mock<IGarmentUnitReceiptNoteFacade>();
            mockFacade.Setup(x => x.ReadForUnitDOHeader(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception(""));

            var mockMapper = new Mock<IMapper>();

            GarmentUnitReceiptNoteController controller = new GarmentUnitReceiptNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetForUnitDOHeader();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
