FROM ubuntu:18.04
RUN apt-get update && \
    apt-get install -y curl && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*
WORKDIR /unity
COPY Builds/Server/ ./
RUN chmod +x UnityServer.x86_64
CMD ["./UnityServer.x86_64", "-batchmode", "-nographics"]