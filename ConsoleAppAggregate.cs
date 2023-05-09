
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace ConsoleAppXrm
{

    internal class ConsoleAppD365
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
                oServiceProxy = service.OrganizationWebProxyClient != null ?
                       service.OrganizationWebProxyClient :
                        (IOrganizationService)service.OrganizationServiceProxy;

                Console.WriteLine("Validating Connection");

                if (oServiceProxy != null)
                {
                    //Get the current user ID:
                    Guid userid = ((WhoAmIResponse)oServiceProxy.Execute(new WhoAmIRequest())).UserId;

                    if (userid != Guid.Empty)
                    {
                        Console.WriteLine("Connection Successful!");
                        Console.WriteLine();
                        //business logic goes here...


                        /*
                        Entity contact = new Entity("contact");
                        contact.Attributes.Add("lastname", "123");

                        Guid guid = service.Create(contact);

                        Console.WriteLine(guid.ToString());
                        */
                        /*-----------------------------------------------------Total email count that is not null--------------------------------------------------------------------------------------*/
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
                            var emailCount = ((AliasedValue)result.Attributes["emailCount"]).Value.ToString();
                            Console.WriteLine($"Total email acount that is not null: {emailCount}");
                        }

                        Console.WriteLine();

                        /*-----------------------------------------------------Total Oppty Est Revenue records--------------------------------------------------------------------------------------------*/
                        string query1 = @"<fetch aggregate='true' >
                                              <entity name='opportunity' >
                                                <attribute name='estimatedvalue' aggregate='sum' alias='totalrevenue' />
                                              </entity>
                                            </fetch>";

                        EntityCollection results1 = service.RetrieveMultiple(new FetchExpression(query1));

                        foreach (var result in results1.Entities)
                        {
                            var returnResult = ((Money)((AliasedValue)result["totalrevenue"]).Value).Value;
                            Console.WriteLine($"Total Oppty Est. Revenue: {returnResult.ToString("c2")}");
                        }


                        Console.WriteLine();

                        /*---------------------------------------------------------Loop through contact record--------------------------------------------------------------------------------------------*/
                        var query2 = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' no-lock='false' distinct='true'>
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


                        //Return all of the contact that has null value of emails
                        EntityCollection results2 = service.RetrieveMultiple(new FetchExpression(query2));

                        Console.WriteLine("Contact emailaddress that is not null");
                        for (int i = 0; i < results2.Entities.Count; i++)
                        {
                            Console.WriteLine($"Index:{i} {results2[i].Attributes["fullname"]}");
                        }

                        /*
                       foreach (var result in results2.Entities)
                       {
                           Console.WriteLine($"Index:{index += 1} {result.Attributes["fullname"].ToString()}");
                       }
                       */

                        /*------------------------------------------------------Create new Oppty and task for the oppty created-------------------------------------------------------------------------*/
                        /*
                        var oppty = new Entity("opportunity");
                        oppty.Attributes.Add("name", "This is my new topic 1");
                        Guid guidOppty = service.Create(oppty);
                        Console.WriteLine($"Oppty is created! Id is: {guidOppty.ToString()}");
                        Console.WriteLine($"Oppty logical name: {oppty.LogicalName}");
                        Console.WriteLine($"Oppty id: {guidOppty}");


                        var task = new Entity("task");
                        task.Attributes.Add("regardingobjectid", new EntityReference(oppty.LogicalName, guidOppty));
                        task.Attributes.Add("subject", $"New task is created for oppty {oppty.Attributes["name"].ToString()}");

                        Guid guidTask = service.Create(task);
                        Console.WriteLine($"Task is created! Id is: {guidTask.ToString()}");
                        */

                        /*------------------------------------------------------Retrieve Oppty record and update-------------------------------------------------------------------------*/
                        Entity opptyRecord = service.Retrieve("opportunity", new Guid("3cbbd39d-d3f0-ea11-a815-000d3a33f3c3"), new ColumnSet("name", "parentcontactid", "description", "msdyn_forecastcategory", "purchaseprocess"));
                        Console.WriteLine(opptyRecord.Attributes["name"].ToString());
                        Console.WriteLine(opptyRecord.Attributes["description"].ToString());
                        Console.WriteLine(((EntityReference)opptyRecord.Attributes["parentcontactid"]).Name);
                        Console.WriteLine(opptyRecord.FormattedValues["msdyn_forecastcategory"].ToString());
                        Console.WriteLine(opptyRecord.FormattedValues["purchaseprocess"].ToString());


                        if (opptyRecord.Attributes.Contains("purchaseprocess"))
                            opptyRecord.Attributes["purchaseprocess"] = new OptionSetValue(0);//individual

                        service.Update(opptyRecord);
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

