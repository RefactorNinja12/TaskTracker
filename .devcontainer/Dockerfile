FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /workspace
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"
CMD ["sleep", "infinity"]