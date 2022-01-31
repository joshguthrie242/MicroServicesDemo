using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using PlatformService;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }
        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            Console.WriteLine($"--> Calling Grpc Service {_configuration["GrpcPlatform"]}");      
            
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var httpHandler = new HttpClientHandler();
            // Return `true` to allow certificates that are untrusted/invalid
            httpHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            //var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"], new GrpcChannelOptions { HttpHandler = httpHandler }); 
            var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                 var reply = client.GetAllPlatforms(request);
                 return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (System.Exception ex)
            {
                
                Console.WriteLine($"--> Could not call Grpc Service {ex.Message}");
                return null;
            }
        }
    }
}