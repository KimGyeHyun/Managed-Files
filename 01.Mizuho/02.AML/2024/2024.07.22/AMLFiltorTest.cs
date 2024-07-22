using AMLFilteringService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using proto_AML;

namespace _AMLTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SingleFilteringPass()
        {
            try
            {
                AMLSingleFilteringReq oRequest = new AMLSingleFilteringReq();
                List<AlgorithmInfo> oAlgoList = new List<AlgorithmInfo>();
                List<string> oNameList = new List<string>();

                // 내가 나에게 보내도록 등록한 후 테스트를 수행하도록 하자.
                Channel channel;
                List<ChannelOption> options = new List<ChannelOption>();
                ChannelOption option = new ChannelOption("grpc.max_receive_message_length", 100 * 1024 * 1024);
                options.Add(option);

                channel = new Channel("172.16.33.77", 8095, ChannelCredentials.Insecure, options);
                var stub = new AMLFiltering.AMLFilteringClient(channel);

                // 값을 채우자!!
                StreamReader sr = new StreamReader("D:\\01. Project\\AML\\Filtor_TestData\\UserList.txt");

                string? line = sr.ReadLine();

                while (line != null)
                {
                    // 정규식을 미리 변경하여 넣도록 한다.
                    line = line.ToUpper();
                    oNameList.Add(line);

                    line = sr.ReadLine();
                }

                sr.Close();

                AlgorithmInfo Jaro = new AlgorithmInfo();
                Jaro.AlgorithmID = 1;
                Jaro.AlgorithmRating = 90;

                AlgorithmInfo Lenven = new AlgorithmInfo();
                Lenven.AlgorithmID = 2;
                Lenven.AlgorithmRating = 1;

                AlgorithmInfo Soundex = new AlgorithmInfo();
                Soundex.AlgorithmID = 3;
                Soundex.AlgorithmRating = 1;

                oAlgoList.Add(Jaro);
                //oAlgoList.Add(Lenven);
                //oAlgoList.Add(Soundex);

                // 값을 다 채웠다!!

                oRequest.Seq = 1;
                oRequest.AlgorithmList.Add(oAlgoList);
                oRequest.UserName.Add(oNameList);

                AMLSingleFilteringRes Res = stub.AMLSingleFiltering(oRequest);

                // 결과를 찍는다.
                Console.WriteLine("Result Seq : " + Res.Seq);
                Console.WriteLine("Result ResultCode : " + Res.ResultCode);
                Console.WriteLine("Result ErrorString : " + Res.ErrorString);
                foreach (FilteringResult o in Res.Result)
                {
                    Console.WriteLine(o.Algorithm + " " + o.UserName + " " + o.SancName + " " + o.Reference + " " + o.Rating);
                }
                Console.WriteLine("Result : ");
            }
            catch (Exception ex)
            {
                Console.WriteLine(" // Exception :  " + ex.Message);
            }

            Assert.Pass();
        }

        [Test]
        public void BatchFilteringPass()
        {
            try
            {
                AMLBatchFilteringReq oRequest = new AMLBatchFilteringReq();

                // 검사 결과 파일명을 등록한다.
                Channel channel;
                channel = new Channel("172.16.33.77", 8095, ChannelCredentials.Insecure);
                var stub = new AMLFiltering.AMLFilteringClient(channel);

                List<AlgorithmInfo> oAlgoList = new List<AlgorithmInfo>();

                oRequest.CPURating = 70;
                AlgorithmInfo Jaro = new AlgorithmInfo();
                Jaro.AlgorithmID = 1;
                Jaro.AlgorithmRating = 90;

                AlgorithmInfo Lenven = new AlgorithmInfo();
                Lenven.AlgorithmID = 2;
                Lenven.AlgorithmRating = 1;

                AlgorithmInfo Soundex = new AlgorithmInfo();
                Soundex.AlgorithmID = 3;
                Soundex.AlgorithmRating = 1;

                oAlgoList.Add(Jaro);
                oAlgoList.Add(Lenven);
                oAlgoList.Add(Soundex);

                oRequest.AlgorithmList.Add(oAlgoList);

                var Res = stub.AMLBatchFiltering(oRequest);

                Console.WriteLine("Result : " + Res.ResultCode + " | " + Res.ErrorString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" // Exception :  " + ex.Message);
            }

            Assert.Pass();
        }
    }
}