dotnet restore DocuFlow.sln
dotnet build DocuFlow.sln -c Release
# TODO: build/push API image, deploy Lambda packages, kubectl apply and frontend S3 sync.
