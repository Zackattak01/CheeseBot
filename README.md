# CheeseBot
![Line Count](https://img.shields.io/tokei/lines/github/Zackattak01/CheeseBot?style=for-the-badge)

Just a bot made to kill time and practice.

## Example Config

```json
{
  "discord": {
    "token": "Your Discord Token"
  },
  
  "postgres": {
    "connection_string": "Your connection string"
  }
}
```

## Docker

To run the docker image a `config.json` must be mounted. For full functionality a proper timezone must be set.  To use plugins mount a `Plugins` folder.

```
$ docker run -d \
    --name cheesebot \
    -v /path/to/config.json:/App/config.json
    -v /path/to/Plugins:/App/Plugins
    -e "TZ=America/New_York"
    zbroderson/cheesebot
```
