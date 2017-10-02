# QuakeLogParser
Obtém as informações do arquivo games.log do jogo Quake Arena 3, agrupa as informações de morte dos players de cada jogo e salva estas informações no banco de dados.

## Pré-requisitos

- Visual Studio [https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community&rel=15#]
- MySql Server 5.6, MySQL Workbench 6.3, MySql Conector 6.9.9 [https://dev.mysql.com/downloads/windows/installer/5.6.html] 

## O que foi feito
Criei uma API utilizando ASP.NET WEB API que contém uma rota que retorna os dados do arquivo, e outra rota que cadastra os dados do arquivo no banco e os retorna em formato JSON.
Para facilitar, criei uma página HTML simples que consome essas duas rotas e apresenta os dados obtidos.

## Rotas

- **Obter os dados do arquivo:** (Para utilizar esta rota, não é necessário criar o banco de dados.)
>`GET [seu localhost]/api/v1/games/file`


- **Obter os dados do banco de dados:**
>`GET [seu localhost]/api/v1/games/bd`

## Testando a API
Para rodar o projeto e testar, após a instalação dos pré-requisitos, é necessário criar o banco de dados utilizando o arquivo **QuakeModeloScritp** localizado na pasta **BD**.
Após a criação do banco de dados, é necessário alterar as informações da **ConnectionString** presente no arquivo **Web.config**, colocando as informações do seu banco de dados.
**Sugestão:** Utilize o Postman [https://www.getpostman.com/] para testar as rotas da API.

## Utilizando App
Abra o arquivo **AppQuakeLogParser** localizado na pasta **App**. Para utilizá-la, é necessário apenas alterar as rotas de acesso conforme o seu **localhost**.

