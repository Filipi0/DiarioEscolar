DiarioEscolar - Sistema de Gestão Educacional

Esta é uma aplicação web de protótipo desenvolvida em Blazor Server para gestão educacional,
com foco em gestão de turmas, controle de estudantes, diários de classe e horários. 
O projeto prioriza a clareza do código e a organização para demonstrar a implementação de um fluxo funcional mínimo e estável.



Arquitetura e Padrões de Design
A arquitetura do sistema é baseada em padrões de design sólidos, como a separação de responsabilidades:


Models: Representam as entidades de negócio da aplicação (ex: Turma, Estudante, Diario).


DTOs: Objetos de transferência de dados, otimizados para a interface do usuário (ex: NovoHorarioDto, DiarioComTurmaDto).

Services: Contêm a lógica de negócio e gerenciam a persistência dos dados.

A persistência de dados é gerenciada por um serviço que utiliza o localStorage do navegador, garantindo que os dados não se percam ao recarregar a página.


Funcionalidades Implementadas
O projeto inclui três áreas principais com funcionalidades essenciais:


Dashboard (Escola): Exibe um resumo geral da escola, com cards que mostram a quantidade de turmas e estudantes por série. Os dados são atualizados dinamicamente com base nas informações cadastradas.




Gestão de Turmas: A interface permite a listagem de turmas com detalhes como nome, série e número de estudantes. É possível criar novas turmas e acessar a tela de detalhes para gerenciar horários, diários e a lista de estudantes.




Diário de Classe: O fluxo inicia com a seleção de um educador, seguido pela exibição dos diários correspondentes. Em um diário específico, um calendário mostra apenas os dias com aula, permitindo o registro de conteúdo e observações para a data selecionada. A lista de estudantes é exibida com base na turma do diário.




Execução do Projeto
Para executar o protótipo localmente, siga os passos abaixo:

Clone o repositório: git clone https://github.com/Filipi0/DiarioEscolar.git

Navegue até a pasta do projeto: cd DiarioEscolar (se não já estiver na pasta)

Execute a aplicação: dotnet watch
