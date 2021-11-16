# this assumes you already built the app as Release configuration
FROM wildernesslabs/m4l-pi as meadow

COPY bin/Release/net5.0/App.dll /m4l/
COPY bin/Release/net5.0/App.runtimeconfig.json /m4l/
COPY bin/Release/net5.0/Bme280.dll /m4l/
COPY bin/Release/net5.0/Meadow.Foundation.dll /m4l/

WORKDIR /m4l

ENTRYPOINT ["dotnet", "App.dll"]
