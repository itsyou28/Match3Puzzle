# Match3Puzzle

# 3매치 퍼즐 스테이지를 편집하고 플레이 합니다.
- Editor를 선택해서 편집할 스테이지를 선택하거나 생성합니다. 
- Stage를 통해서 편집한 스테이지를 플레이 합니다. 
- 왼쪽의 Field/Block 모드를 통해 편집모드를 변경할 수 있습니다. 
- Editor 모드에서 필드영역을 드래그하면 서클 메뉴가 출현합니다. 
- 필드 편집모드에서는 선택된 필드의 플레이가능여부, 생성필드, 진행방향 을 지정할 수 있습니다. 
- 블럭 편집모드에서는 선택된 필드를 특정 블럭으로 시작하도록 지정할 수 있습니다. 



## 주요 클래스

### BlockField 
블럭이 배치 가능한 영역. 블럭의 이동방향, 생성여부 등의 속성을 지정할 수 있다. 
### Block 
 플레이어가 컨트롤하는 박스. 박스의 속성, 위치정보, 현재 위치 필드, 애니메이션 등을 관리한다. 
### BlockFieldMng 
 전체BlockField를 배열로 관리하면서, 스테이지의 초기화/매칭/블럭Swap등 필드의 상대관계에 기반한 기능들을 처리한다. 
### BlockGO 
 게임오브젝트 유니티 씬에 올려지는 블럭 이미지 컨테이너. Block의 호출에 따라 블럭의 실제 이동/매치/소멸 등의 애니메이션 등 블럭이 실제 화면에 연출되는 부분을 처리한다. 
### BlockFieldGO 
 게임오브젝트. 유니티 씬에 올려지는 필드 이미지 컨테이너. Field에 관계된 화면 연출 부분을 처리한다. 
### StageManager 
 StageMode에서 BlockFieldMng와 유니티 엔진의 Adapter 역할을 한다. 
### EditManager 
 EditMode에서에서 BlockFieldMng와 유니티 엔진의 Adapter 역할을 한다. 
