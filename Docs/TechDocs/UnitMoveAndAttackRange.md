# 유닛 이동과 공격 범위

## 📝 1. 기능 개요

### 1.1. 기능 정의

- 선택한 유닛이 이동 가능한 범위, 공격 가능한 범위를 표시

- 유닛의 이동력, 지형의 종류를 고려하여 이동 가능 범위와 경로 계산

### 1.2. 구현 목표

- 다익스트라를 이용해 최단 경로를 계산하고, 이동 가능한 지점을 표시

- 실제 유닛을 이동시킬 때 찾은 최단 경로를 따라 이동하도록 각 지점의 경로 저장

- 공격 범위의 모양이 없으며 사거리만 존재한다면 사거리 내의 좌표 저장

---

## 🛠️ 2. 작성 클래스 설계 및 구조 

### 🔗 2.1. 클래스 간 관계도

## 핵심 클래스 세부 역할

### `TileRange.cs`

- **유닛 이동 가능 범위 선택 (`UnitMoveRange()`)**: 유닛의 현재 좌표를 시작점으로 삼아 다익스트라로 주변 타일의 최단 경로를 계산하여 이동 가능 범위를 리스트에 저장

- **사거리가 정해진 무기의 공격 범위 계산 (`SelectTargetWeaponRange()`)**: 타일의 비용을 전부 1로 계산하여 무기 사거리에 해당하는 공격 가능 범위를 리스트에 저장

- **`TileCost`의 거리를 갱신하면 Min Heap 재정렬**: 최단 거리가 바뀌었을 때 새롭게 Min Heap에 대입하는 대신, Min Heap 안에 있는 `TileCost`를 찾아 부모와 비교하여 Min Heap 정렬

- **두 지점의 거리 계산 (`CalculateTileCost()`)**: 두 지점의 최단 거리 계산

<br>

### `TileCost.cs`

- **타일 경로 및 거리 저장**: 타일 좌표와 데이터, 최단 경로의 이전 `TileCost`, 최단 거리를 보관

<br>

### `PriorityQueue.cs`

- **Min Heap에 삽입 (`Add()`)**: Min Heap에 `TileCost`를 삽입하고 정렬

- **Min Heap에서 제거 (`Remove()`)**: Min Heap에서 루트 노드를 제거하여 Min Heap에서 `TileCost`의 거리가 가장 작은 것을 반환

- **부모와 비교 (`CompareWithParent()`)**: 대입한 `TileCost`를 부모 노드와 계속 비교하며 바꿔서 전체 트리가 Min Heap이 되도록 정렬

<br>

---

### 🔄 2.2. 다익스트라와 Min Heap

## 단계별 세부 설명

### 1. `TileCost`의 맵과 PriorityQueue 생성

- 타일 맵 정보, 유닛 맵 정보를 받으면, 맵 크기가 고정되므로 `TileCost` 이차원 배열 생성 가능

- `TileCost`의 거리에 따라 정렬할 `PriorityQueue` 생성

<br>

### 2. 이동 가능 범위 계산 (`UnitMoveRange()`)

- 유닛의 시작 지점부터 `PriorityQueue`에 넣어, 방문하지 않은 이웃들의 최단 거리를 계산

- 계산한 거리가 최단 거리였다면, `PriorityQueue`에 이미 존재할 경우 거리를 갱신하고 재정렬(`PriorityQueue.CompareWithParent()`), 넣지 않았다면 삽입

- `PriorityQueue`에 최단 거리 노드를 제거하여 `PriorityQueue`가 비거나 꺼낸 TileCost의 거리가 유닛의 이동력보다 큰 경우까지 반복

<br>

### 3. 특정 두 좌표 간의 거리를 계산 (`CalculateTileCost()`)

- 좌표 두 개가 주어졌을 때, 한쪽을 시작점으로 두고 다익스트라를 이용하여 둘 사이의 최단 거리를 계산
---
