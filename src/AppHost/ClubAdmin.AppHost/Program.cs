var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sql")
    .WithDataVolume();

var eventStoreDb = builder.AddContainer("eventstoredb", "eventstore/eventstore")
    .WithEnvironment("EVENTSTORE_CLUSTER_SIZE", "1")
    .WithEnvironment("EVENTSTORE_RUN_PROJECTIONS", "All")
    .WithEnvironment("EVENTSTORE_INSECURE", "true")
    .WithEndpoint(2113, 2113, name: "http")
    .WithEndpoint(1113, 1113, name: "tcp");

var membersApi = builder.AddProject<Projects.ClubAdmin_Members_Api>("members-api")
    .WithReference(sqlServer)
    .WaitFor(sqlServer);

var financesApi = builder.AddProject<Projects.ClubAdmin_Finances_Api>("finances-api")
    .WithReference(sqlServer)
    .WithReference(eventStoreDb)
    .WaitFor(sqlServer)
    .WaitFor(eventStoreDb);

builder.AddNpmApp("club-admin-ui", "../../../frontend/club-admin-ui", "dev")
    .WithReference(membersApi)
    .WithReference(financesApi)
    .WithEnvironment("NODE_ENV", "development")
    .WithHttpEndpoint(env: "PORT");

builder.Build().Run();
