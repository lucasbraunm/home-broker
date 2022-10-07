# home-broker
Aplicação de console em C# que monitora a cotação de um ativo da B3, enviando um e-mail recomendando a compra ou a venda, com base em informações providas pelo usuário.

## Usabilidade
```bash
O input deve ser feito com 3 informações, separadas por espaços em branco, na forma:

> HomeBroker.exe BBAS3 41,40 40,50
(NOME_ATIVO VALOR_MÁXIMO VALOR_MÍNIMO)
```
## API
A cotação do ativo é feito utilizando a API da HG Brasil, encontrada nesse [link](https://console.hgbrasil.com/keys/new_key_plan). É uma API limitada, mas fácil de usar e gratuita. A chave de acesso à API pode ser modificada no arquivo de configurações.

## SMTP Server
É possível utilizar o servidor SMTP de sua escolha. Para isso, customize o arquivo de configurações com os dados do servidor, remetente e destinatário dos envios de email.
