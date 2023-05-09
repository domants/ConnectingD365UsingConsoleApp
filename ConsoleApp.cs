
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace ConsoleAppXrm
{

    internal class AggregateFetchXML
    {
        static void Main(string[] args)
        {
            IOrganizationService oServiceProxy;
            try
            {
                Console.WriteLine("Setting up Dynamics 365 connection");

                string connectionString = "AuthType=OAuth;Username=admin@CRM293873.onmicrosoft.com;Password=uA04vfQT4X;Url=https://org9fd542ea.crm5.dynamics.com;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;LoginPrompt=Auto";

                //Create the Dynamics 365 Connection:
                CrmServiceClient service = new CrmServiceClient(connectionString);

                //Create the IOrganizationService:
                oServiceProxy = (IOrganizationService)service.OrganizationWebProxyClient != null ?
                        (IOrganizationService)service.OrganizationWebProxyClient :
                        (IOrganizationService)service.OrganizationServiceProxy;

                Console.WriteLine("Validating Connection");

                if (oServiceProxy != null)
                {
                    //Get the current user ID:
                    Guid userid = ((WhoAmIResponse)oServiceProxy.Execute(new WhoAmIRequest())).UserId;

                    if (userid != Guid.Empty)
                    {
                        Console.WriteLine("Connection Successful!");

                        //business logic goes here...


                        /*
                        Entity contact = new Entity("contact");
                        contact.Attributes.Add("lastname", "123");

                        Guid guid = service.Create(contact);

                        Console.WriteLine(guid.ToString());
                        */

                        var query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' no-lock='false' distinct='true' aggregate='true'>
                                            <entity name='contact'>
                                                    <attribute name='fullname' alias='fullNameCount' aggregate='count'/>
                                                    <attribute name='emailaddress1' alias='emailCount' aggregate='count'/>
                                                <filter type='and'>
                                                    <condition attribute='emailaddress1' operator='not-null'/>
                                                </filter>
                                            </entity>
                                        </fetch>";

                        EntityCollection results = service.RetrieveMultiple(new FetchExpression(query));

                        foreach (var result in results.Entities)
                        {
                            Console.WriteLine(((AliasedValue)result["emailCount"]).Value.ToString());
                        }


                        /*-----------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
                        var query1 = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' no-lock='false' distinct='true'>
                                        <entity name='contact'>
                                            <attribute name='entityimage_url'/>
                                            <attribute name='fullname'/>
                                            <order attribute='fullname' descending='false'/>
                                            <attribute name='parentcustomerid'/>
                                            <attribute name='telephone1'/>
                                            <attribute name='emailaddress1'/>
                                            <attribute name='contactid'/>
                                        <filter type='and'>
                                            <condition attribute='emailaddress1' operator='not-null'/>
                                        </filter>
                                        </entity>
                                        </fetch>";

                        Console.WriteLine();


                        //Return all of the contact that has null value of emails
                        EntityCollection results1 = service.RetrieveMultiple(new FetchExpression(query1));

                        for (int i = 0; i < results1.Entities.Count; i++)
                        {
                            Console.WriteLine($"Index:{i} {results1[i].Attributes["fullname"]}");
                        }

                        /*
                       foreach (var result in results1.Entities)
                       {
                           Console.WriteLine($"Index:{index += 1} {result.Attributes["fullname"].ToString()}");
                       }
                       */
                    }
                }
                else
                {
                    Console.WriteLine("Connection failed... Please check the credential or Url you insert");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - " + ex.ToString());
            }

            Console.Read();
        }
    }

}

