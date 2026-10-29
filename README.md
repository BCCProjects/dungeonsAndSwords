# Dungeons & Swords

Platformer 2D de visão lateral feito em Unity. O jogador escolhe uma classe (Guerreiro, Arqueiro ou Mago), atravessa três fases derrotando inimigos, evolui de nível e enfrenta o Cavaleiro Sombrio na última fase.

## Equipe

- **Curso:** Ciência da Computação
- **Disciplina:** Desenvolvimento de Jogos Digitais
- **Professor:** Luis Filipe Grael Tinós
- **Integrantes:**
  - Bryan Joab Messias de Souza
  - Davi Guilherme Grigolin
  - Fernando Rafael Lopes Filho
  - Italo Lenharo Thomazete
  - Victor Augusto de Mattos Carbelotti

## Como executar

### Pelo Unity (editor)

1. Instale o **Unity 6.3 LTS (6000.3.25f1)** pelo Unity Hub.
2. No Unity Hub, clique em **Add → Add project from disk** e selecione a pasta deste projeto.
3. Abra o projeto e aguarde a importação inicial.
4. Abra a cena `Assets/Scenes/MainMenu.unity`.
5. Clique em **Play**.

### Gerando o executável

1. No Unity, abra **File → Build Profiles**.
2. Selecione a plataforma **Windows** (as cenas já estão registradas, com `MainMenu` como cena inicial).
3. Clique em **Build** e escolha uma pasta de saída.
4. Execute o `.exe` gerado.

> Os assets de terceiros vêm da Unity Asset Store (lista na tela de Créditos do jogo). Se alguma pasta de asset estiver faltando, importe o pacote pelo **Package Manager → My Assets**.

## Controles

| Ação | Teclado | Controle |
|---|---|---|
| Andar | A / D ou ← / → | Analógico esquerdo |
| Pular | W ou ↑ | A (Xbox) |
| Atacar | Espaço ou clique esquerdo | X (Xbox) |
| Pausar | Esc | Start |

## Como jogar

- Derrote **todos os inimigos** da fase para concluí-la. Antes de avançar, escolha os atributos do level up e colete os itens que caíram.
- A cada nível escolha um atributo: **Vida**, **Força**, **Agilidade** ou **Resistência**.
- Os inimigos podem soltar **Orbe de Experiência**, **Poção de Cura** e poções temporárias de **Força**, **Agilidade** e **Resistência** (30 segundos).
- Na fase 3, depois de limpar a caverna, o **Cavaleiro Sombrio** aparece na arena. Derrote-o para vencer.

## Estrutura do projeto

| Pasta | Conteúdo |
|---|---|
| `Assets/Scenes` | MainMenu, Credits, ClassSelect, Level1–3, Victory, GameOver |
| `Assets/Scripts` | Código do jogo (Core, Player, Combat, Enemies, Items, UI, World, Data) |
| `Assets/Prefabs` | Jogadores, inimigos, chefe, projéteis, itens, UI e sistemas |
| `Assets/ScriptableObjects` | Dados das classes, inimigos e itens |
| `Assets/Animations` | Clips e Animator Controllers |
| `Assets/Tilemaps` | Tiles e Tile Palettes das fases |
| `Assets/Art`, `Assets/Audio`, `Assets/UI` | Arte, efeitos sonoros e fonte |
| `Assets/ThirdParty` | Pacotes da Asset Store: `Characters` (personagens e chefe), `Environments` (tilesets e fundos), `Fonts`, `Audio` (música) |
