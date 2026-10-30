FROM alpine:latest AS build

RUN apk add --no-cache \
  musl-dev \
  gcc

WORKDIR /src

COPY main.c .

RUN mkdir -p /out \
  && gcc \
    -std=c11 \
    -Wall \
    -Wextra \
    -Os \
    -static \
    -s \
    main.c \
    -o /out/ssh-passphrase

FROM scratch
COPY --from=build /out/ssh-passphrase /ssh-passphrase

# Use one of the two commands to create:
# docker buildx build --platform linux/amd64 --output type=local,dest=./out .
# mkdir -p ./out && docker buildx build --platform linux/amd64 --output type=tar,dest=- . | tar -xf - -C ./out/
