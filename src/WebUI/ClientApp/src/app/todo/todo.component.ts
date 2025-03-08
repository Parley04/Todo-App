import { Component, TemplateRef, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import {
  TodoListsClient, TodoItemsClient,
  TodoListDto, TodoItemDto, PriorityLevelDto,
  CreateTodoListCommand, UpdateTodoListCommand,
  CreateTodoItemCommand, UpdateTodoItemDetailCommand,
  Colour,
  TagsClient,
  Tag,
  ItemTagsClient,
  ItemTag,
  TodoItem,
} from '../web-api-client';
import { ColourDto } from './todo-list/models/colours';
import { AuthorizeService } from 'src/api-authorization/authorize.service';

@Component({
  selector: 'app-todo-component',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.scss']
})


export class TodoComponent implements OnInit {
  debug = false;
  deleting = false;
  deleteCountDown = 0;
  deleteCountDownInterval: any;
  lists: TodoListDto[];
  priorityLevels: PriorityLevelDto[];
  //colours: Colour[];
  selectedList: TodoListDto;
  selectedItem: TodoItemDto;
  newListEditor: any = {};
  listOptionsEditor: any = {};
  newListModalRef: BsModalRef;
  listOptionsModalRef: BsModalRef;
  itemTagOptionsModalRef: BsModalRef;
  tagListModalRef: BsModalRef;
  deleteListModalRef: BsModalRef;
  deleteItemTagModalRef: BsModalRef;
  itemDetailsModalRef: BsModalRef;
  itemDetailsFormGroup = this.fb.group({
    id: [null],
    listId: [null],
    priority: [''],
    note: ['']
  });

  colours: ColourDto[] = [
    { value: "#FFFFFF", name: "White" },
    { value: "#FF5733", name: "Red" },
    { value: "#FFC300", name: "Orange" },
    { value: "#FFFF66", name: "Yellow" },
    { value: "#CCFF99", name: "Green" },
    { value: "#6666FF", name: "Blue" },
    { value: "#9966CC", name: "Purple" },
    { value: "#999999", name: "Grey" },
  ]

  constructor(
    private listsClient: TodoListsClient,
    private tagsClient: TagsClient,
    private itemTagClient: ItemTagsClient,
    private authService: AuthorizeService,
    private itemsClient: TodoItemsClient,
    private modalService: BsModalService,
    private fb: FormBuilder
  ) { }

  userId: string;
  tag: Tag = new Tag();

  myTags: Tag[] = [];
  unchosenTags: Tag[] = [];
  itemTag: ItemTag = new ItemTag();
  chosenItemTag: any;
  displayedTags: any;
  displayedMyTags: any;
  filterText: string = "";
  filterTextList: string = "";
  selectedItemTagId: number;
  isMyTag: boolean = true;
  tempColour:any;
  todoItem:TodoItem=new TodoItem();

  ngOnInit(): void {
    this.authService.getUser().subscribe((res: any) => {
      this.userId = res.sub;
      this.getAll();
    })
  }
  getAll() {
    this.listsClient.getList(this.userId).subscribe(
      result => {
        this.lists = result.lists;
  
        // Her list öğesi için renk adını atamak
        // this.lists.forEach(list => {
        //   list.colour.code = this.getColourName(list.colour.code);
        // });
  
        console.log(this.lists);  // Artık her liste öğesinde colourName özelliği var
        this.priorityLevels = result.priorityLevels;
  
        if (this.lists.length) {
          this.selectedList = this.lists[0];
        }
      },
      error => console.error(error)
    );
  }
  
  getColourName(colourCode: string): string {
    const colour = this.colours.find(c => c.value === colourCode);
    return colour ? colour.name : "Unknown";  // Eşleşme varsa renk adı, yoksa "Unknown"
  }
  
  openTagList(template: TemplateRef<any>, todoItemId: number) {
    this.selectedItemTagId = todoItemId;

    this.tagsClient.getChosenTags(this.userId, todoItemId).subscribe((res: any) => {
      this.myTags = res;

      this.displayedMyTags = [...this.myTags];
      this.tagListModalRef = this.modalService.show(template);
      this.isMyTag = true;
    })
  }









  remainingItems(list: TodoListDto): number {
    return list.items.filter(t => !t.done).length;
  }

  showNewListModal(template: TemplateRef<any>): void {
    this.newListModalRef = this.modalService.show(template);
    setTimeout(() => document.getElementById('title').focus(), 250);
  }
  showItemTagDetailsModal(template: TemplateRef<any>, item: any): void {

  }

  newListCancelled(): void {
    this.newListModalRef.hide();
    this.newListEditor = {};
  }

  addList(): void {
    const list = {
      id: 0,
      title: this.newListEditor.title,
      userId: this.userId,
      colour:{},
      items: []
    } as TodoListDto;
    this.listsClient.create(list as CreateTodoListCommand).subscribe(
      result => {
        list.id = result;
        this.lists.push(list);
        this.selectedList = list;
        this.tempColour=this.colours[0];
        this.newListModalRef.hide();
        this.newListEditor = {};
      },
      error => {
        const errors = JSON.parse(error.response);
        if (errors && errors.Title) {
          this.newListEditor.error = errors.Title[0];
        }

        setTimeout(() => document.getElementById('title').focus(), 250);
      }
    );
  }
  showListOptionsModal(template: TemplateRef<any>) {
    this.listOptionsEditor = {
      id: this.selectedList.id,
      title: this.selectedList.title,
      colour: this.selectedList.colour
    };
    this.tempColour=this.getColourName(this.selectedList.colour.code);
    this.listOptionsModalRef = this.modalService.show(template);
  }
  todoList:TodoListDto=new TodoListDto();
  updateListOptions() {
    const list = this.listOptionsEditor as UpdateTodoListCommand;
    list.colour=this.tempColour;
    list.userId=this.userId;
    console.log(list);
    this.listsClient.update(this.selectedList.id, list).subscribe(
      () => {
        (this.selectedList.title = this.listOptionsEditor.title),
          this.listOptionsModalRef.hide();
        this.listOptionsEditor = {};
        this.getAll();
      },
      error => console.error(error)
    );
  }

  confirmDeleteList(template: TemplateRef<any>) {
    this.listOptionsModalRef.hide();
    this.deleteListModalRef = this.modalService.show(template);
  }

  deleteListConfirmed(): void {
    this.listsClient.delete(this.selectedList.id).subscribe(
      () => {
        this.deleteListModalRef.hide();
        this.lists = this.lists.filter(t => t.id !== this.selectedList.id);
        this.selectedList = this.lists.length ? this.lists[0] : null;
      },
      error => console.error(error)
    );
  }

  showItemDetailsModal(template: TemplateRef<any>, item: TodoItemDto): void {
    this.getUnchosenTagList(item);

    this.selectedItemTagId=item.id;
    this.selectedItem = item;
    this.itemDetailsFormGroup.patchValue(this.selectedItem);

    this.itemDetailsModalRef = this.modalService.show(template);
    this.itemDetailsModalRef.onHidden.subscribe(() => {
      this.stopDeleteCountDown();
    });
  }

  getUnchosenTagList(item: any) {
    this.tagsClient.getUnchosenTags(this.userId, item.id).subscribe((res: any) => {
      this.unchosenTags = res;
      this.displayedTags = [...this.unchosenTags];
      this.isMyTag = false;
    });
  }

  updateItemDetails(): void {
    const item = new UpdateTodoItemDetailCommand({
      id: this.selectedItem.id,
      listId: this.selectedItem.listId,
      priority: Number(this.selectedItem.priority),
      note: this.selectedItem.note
    });
    this.itemsClient.updateItemDetails(this.selectedItem.id, item).subscribe(
      () => {
        if (this.selectedItem.listId !== item.listId) {
          this.selectedList.items = this.selectedList.items.filter(i => i.id !== this.selectedItem.id);
          const listIndex = this.lists.findIndex(l => l.id === item.listId);
          this.selectedItem.listId = item.listId;
          this.lists[listIndex].items.push(this.selectedItem);
        }
        this.selectedItem.priority = item.priority;
        this.selectedItem.note = item.note;
        this.getAll();
        this.itemDetailsModalRef.hide();
      },
      error => console.error(error)
    );
  }
  
  saveTag() {
    this.tag.created = new Date();
    this.tag.userId = this.userId;
    this.tag.countUses = 0;
    this.tagsClient.create(this.tag).subscribe((res: any) => {
      this.saveItemTag(res);
    })
  }

  saveItemTag(tagId: number) {
    this.itemTag.tagId = tagId;
    this.itemTag.todoItemId = this.selectedItemTagId;
    this.itemTagClient.create(this.itemTag).subscribe((res: any) => {
      if (this.isMyTag == true) {
        this.tagListModalRef.hide();
      }
      else {
        this.itemDetailsModalRef.hide();
      }
      this.itemDetailsFormGroup.reset();
      this.getAll();
    })
  }


  filterTags() {
    const query = this.tag.name.toLowerCase();
    if (query) {
      this.displayedTags = this.unchosenTags.filter(tag => tag.name.toLowerCase().includes(query));
    } else {
      this.displayedTags = [...this.unchosenTags]; 
    }
  }
  filterMyTags() {
    const query = this.tag.name.toLowerCase();
    if (query) {
      this.displayedMyTags = this.myTags.filter(tag => tag.name.toLowerCase().includes(query));
    } else {
      this.displayedMyTags = [...this.myTags];
    }
  }
  addItem() {
    const item = {
      id: 0,
      listId: this.selectedList.id,
      priority: this.priorityLevels[0].value,
      title: '',
      done: false
    } as TodoItemDto;

    this.selectedList.items.push(item);
    const index = this.selectedList.items.length - 1;
    this.editItem(item, 'itemTitle' + index);
  }

  editItem(item: any, elementId: string): void {
    this.selectedItem = item; 
  
    setTimeout(() => {
      const inputElement = document.getElementById(elementId) as HTMLInputElement;
      if (inputElement) {
        inputElement.focus();
      }
    }, 0); 
  }
  
  updateItem(item: TodoItemDto, pressedEnter: boolean = false): void {
    const isNewItem = item.id === 0;

    if (!item.title.trim()) {
      this.deleteItem(item);
      return;
    }
    if (item.id === 0) {
      this.itemsClient
        .create({
          ...item, listId: this.selectedList.id
        } as CreateTodoItemCommand)
        .subscribe(
          result => {
            item.id = result;
          },
          error => console.error(error)
        );
    } else {
      this.itemsClient.update(item.id, item).subscribe(
        () => console.log('Update succeeded.'),
        error => console.error(error)
      );
    }
    this.selectedItem = null;

    if (isNewItem && pressedEnter) {
      setTimeout(() => this.addItem(), 250);
    }
  }


  choseItemTag(template: TemplateRef<any>, itemTag: ItemTag) {
    this.itemTagOptionsModalRef = this.modalService.show(template);
    this.chosenItemTag = itemTag;
  }

  deleteItemTagWithIds(itemId:number, tagId:number){
    this.itemTagClient.delete2(itemId, tagId).subscribe((res: any) => {
      this.tagListModalRef.hide();
      this.getAll();
    })
  }
  deleteItemTag() {
    this.itemTagClient.delete(this.chosenItemTag.id).subscribe((res: any) => {
      this.itemTagOptionsModalRef.hide();
      this.getAll();
    })
  }


  deleteItem(item: TodoItemDto, countDown?: boolean) {
    if (countDown) {
      if (this.deleting) {
        this.stopDeleteCountDown();
        return;
      }
      this.deleteCountDown = 3;
      this.deleting = true;
      this.deleteCountDownInterval = setInterval(() => {
        if (this.deleting && --this.deleteCountDown <= 0) {
          this.deleteItem(item, false);
        }
      }, 1000);
      return;
    }
    this.deleting = false;
    if (this.itemDetailsModalRef) {
      this.itemDetailsModalRef.hide();
    }

    if (item.id === 0) {
      const itemIndex = this.selectedList.items.indexOf(this.selectedItem);
      this.selectedList.items.splice(itemIndex, 1);
    } else {
      this.itemsClient.delete(item.id).subscribe(
        () =>
        (this.selectedList.items = this.selectedList.items.filter(
          t => t.id !== item.id
        )),
        error => console.error(error)
      );
    }
  }

  stopDeleteCountDown() {
    clearInterval(this.deleteCountDownInterval);
    this.deleteCountDown = 0;
    this.deleting = false;
  }
}