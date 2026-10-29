# Dungeons & Swords

Platformer 2D de visão lateral feito em Unity. O jogador escolhe uma classe (Guerreiro, Arqueiro ou Mago), atravessa cinco fases derrotando inimigos, evolui de nível e enfrenta três chefes: o Cavaleiro Sombrio (fase 3), o Bringer of Death (fase 4) e o Demon Slime na fase final.

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

> Os assets de terceiros já estão no projeto, em `Assets/ThirdParty` (lista abaixo e na tela de Créditos do jogo). A maioria vem da Unity Asset Store: se alguma pasta estiver faltando, importe o pacote pelo **Package Manager → My Assets**. O Flying Demon e o Boss Demon Slime são gratuitos e foram baixados fora da loja.

## Controles

| Ação   | Teclado                   |
| ------ | ------------------------- |
| Andar  | A / D ou ← / →            |
| Pular  | W ou ↑                    |
| Atacar | Espaço ou clique esquerdo |
| Pausar | Esc                       |

## Como jogar

- Derrote **todos os inimigos** da fase para concluí-la. Antes de avançar, escolha os atributos do level up e colete os itens que caíram.
- A cada nível escolha um atributo: **Vida**, **Força**, **Agilidade** ou **Resistência**.
- Os inimigos podem soltar **Orbe de Experiência**, **Poção de Cura** e poções temporárias de **Força**, **Agilidade** e **Resistência** (30 segundos).
- Na fase 3, depois de limpar a caverna, o **Cavaleiro Sombrio** aparece na arena.
- Na fase 4, depois de limpar o mapa, surge o **Bringer of Death**: golpes de foice de perto e magias que abrem um portal no chão sob o jogador (três de uma vez quando ele fica com metade da vida).
- Na fase 5, nas muralhas do castelo, aparecem os **Demônios Voadores**, que atiram bolas de fogo de longe. No salão final está o chefe final, o **Demon Slime**: golpe de cutelo de perto e ondas de fogo rente ao chão (pule por cima; duas seguidas quando ele fica com metade da vida). Derrote-o para vencer.

## Estrutura do projeto

| Pasta                                     | Conteúdo                                                                                                                 |
| ----------------------------------------- | ------------------------------------------------------------------------------------------------------------------------ |
| `Assets/Scenes`                           | MainMenu, Credits, ClassSelect, Level1–5, Victory, GameOver                                                              |
| `Assets/Scripts`                          | Código do jogo (Core, Player, Combat, Enemies, Items, UI, World, Data)                                                   |
| `Assets/Prefabs`                          | Jogadores, inimigos, chefe, projéteis, itens, UI e sistemas                                                              |
| `Assets/ScriptableObjects`                | Dados das classes, inimigos e itens                                                                                      |
| `Assets/Animations`                       | Clips e Animator Controllers                                                                                             |
| `Assets/Tilemaps`                         | Tiles e Tile Palettes das fases                                                                                          |
| `Assets/Art`, `Assets/Audio`, `Assets/UI` | Arte, efeitos sonoros e fonte                                                                                            |
| `Assets/ThirdParty`                       | Pacotes de terceiros: `Characters` (personagens e chefes), `Environments` (tilesets e fundos), `Fonts`, `Audio` (música) |

## Créditos dos assets

| Uso no jogo                 | Asset                                     | Autor            |
| --------------------------- | ----------------------------------------- | ---------------- |
| Fundos dos menus            | Animated Pixel-Art Backgrounds            | Feony Labs       |
| Fase 1 (Floresta)           | Pixel Art Woods Tileset and Background    | karsiori         |
| Fase 2 (Castelo)            | Medieval pixel art asset FREE             | Blackspire       |
| Fase 3 (Caverna)            | 2D Platfrom Tile Set - Cave               | Iphigenia Pixels |
| Fase 4 (Caverna final)      | Pixel Fantasy Caves                       | Szadi Art.       |
| Fase 5 (Castelo do Demônio) | Platformer Set                            | Szadi Art.       |
| Guerreiro                   | Hero Knight - Pixel Art                   | Sven Thole       |
| Arqueiro e inimigos         | Hero and Opponents Animation              | Szadi Art.       |
| Mago                        | Evil Wizard                               | Luiz Melo        |
| Monstros e voador           | Monsters_Creatures_Fantasy                | Luiz Melo        |
| Chefe da fase 3             | Dark Knight 2D – Animated Character + VFX | TealFalcon       |
| Chefe da fase 4             | Bringer Of Death                          | Clembod          |
| Chefe final                 | Boss Demon Slime                          | chierit          |
| Demônio Voador              | Flying Demon 2D Pixel Art                 | Mattz Art        |
| Música                      | Free 10 Medieval Ambient Fantasy Tracks   | alkakrab         |
| Fonte                       | BoldPixels Font                           | YukiPixels       |
