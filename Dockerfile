FROM mono:6.0 AS build
WORKDIR /build
COPY . /build
RUN cd source && msbuild
