﻿class ChatList {
    RelativeUrl;
    LoggedInUserId;
    constructor(loggedInUserId, relativeUrl = "/") {
        this.RelativeUrl = relativeUrl;
        this.FetchAllChats();
        this.LoggedInUserId = loggedInUserId;
        this.ChatContent = $("#ChatContent")
        this.Chats = new Map();
        var pollLoop = this.InitPollLoop();
        const self = this;
        $(document).ready(() => {
            $("#DeleteUser").click(() => { self.DeleteUser() })
            $("#AdminUser").click(() => { self.AdminUser() })
            $("#LeaveChatButton").click(() => self.LeaveChat());
        });
    }

    Chats;
    ChatHeader;
    ChatContent;
    ChatListContainer = $("#ChatListInnerContainer");
    OpenChat;
    SelectedUser = { element: null, user: null};

    LeaveChat() {
        this.Chats.delete(this.OpenChat.id);
        $(`[chatid='${this.OpenChat.id}']`).remove();
        $("#ChatContent").empty();
        $("#ChatHeaderBar").text(null);
        $.ajax({
            url: `${this.RelativeUrl}Chat/LeaveChat`,
            method: "POST",
            data: { chatId: this.OpenChat.id }
        });
        this.OpenChat = null;
        $("#CloseUserListHeader").click();
    }

    DeleteUser() {
        $("#UserOptionsPopup").hide();
        if (this.SelectedUser.user == null) {
            return;
        }
        $.ajax({
            url: `${this.RelativeUrl}Chat/RemoveUserFromChat`,
            method: "POST",
            data: { chatId: this.OpenChat.id, userId: this.SelectedUser.user.id }
        })
        this.SelectedUser.element.remove();
    }

    AdminUser() {
        $("#UserOptionsPopup").hide();
        if (this.SelectedUser == null) {
            return;
        }

        $.ajax({
            url: `${this.RelativeUrl}Chat/MakeUserAdmin`,
            method: "POST",
            data: { chatId: this.OpenChat.id, userId: this.SelectedUser.user.id }
        })
        $(".UserListAdmin", this.SelectedUser.element).show();
    }

    InitEvents() {
        const self = this;
        $(".ChatOuterContainer").click((s) => {
            self.FetchChatContent(
                $(s.currentTarget).attr("ChatId")
            );
        });

        $("#ChatInputBar").keydown((e) => { if (e.keyCode == 13) self.TrySendMessage(); });
        $("#ChatSendButton").click(() => { self.TrySendMessage(); });
        $("#ChatHeaderBar").click(() => { self.OpenChatSettings() });
        $("#CloseUserListHeader").click(() => { $("#SecondaryContainer").hide() });
        $("#AddUserToChat").click(() => { self.SetAddUserCallback() })
    }

    SetAddUserCallback() {
        const self = this;
        window.ClickOnUserCallback = (user) => { self.AddUserToChat(user) };
    }

    AddUserToChat(user) {
        $("#FindUsersPopup").hide();
        $("#FindUserInput").val(null);
        if (this.OpenChat.users.some(u => u.id == user.id)) {
            $.toast({
                heading: 'Error',
                text: 'Deze gebruiker zit al in de chat',
                showHideTransition: 'fade',
                icon: 'error'
            });
            return;
        }
        this.OpenChat.users.push(user)
        user.isAdmin = false;
        $.ajax({
            url: `${this.RelativeUrl}Chat/AddUser`,
            method: "POST",
            data: { userId: user.id, chatId: this.OpenChat.id },
            success: () => {
                this.RenderUser(user);
            }
        });


    }

    TrySendMessage() {
        let message = $("#ChatInputBar").val();
        $("#ChatInputBar").val(null);
        if (message == "" || message == null) {
            return;
        }
        this.SendMessage(message);
    }

    OpenChatSettings() {
        if (this.OpenChat == null) {
            return; // don't do anything when header clicked
        }
        $("#SecondaryContainer").show();
    }

    SendMessage(message) {
        $.ajax({
            url: `${this.RelativeUrl}Chat/SendMessage`,
            data: { messageContent: message, chatId: this.OpenChat.id },
            method: "POST"
        });
    }

    FetchChatContent(chatId) {
        $.ajax({
            url: `${this.RelativeUrl}Chat/GetChatContent`,
            method: "GET",
            traditional: true,
            data: { chatId: chatId },
            success: (chat) => {
                this.OpenChat = this.Chats.get(chat.id);
                this.OpenChat.users = chat.users;
                this.LoadChatContent(chat);
            }
        });
    }

    FetchAllChats() {
        $.ajax({
            url: `${this.RelativeUrl}Chat/GetUserChats`,
            method: "GET",
            success: (chats) => {
                this.ChatListContainer.empty();
                for (let chat of chats) {
                    this.Chats.set(chat.id, chat);
                    this.ChatListContainer.append(this.RenderChat(chat));
                }
                this.InitEvents();
            }
        });
    }

    InitPollLoop() {
        return setTimeout(() => {
            this.PollChats();
            this.InitPollLoop();
        },
        1000)
    }

    PollChats() {
        let chatPolls = [];
        for (let chatKeyValuePair of chatList.Chats) {
            let chat = chatKeyValuePair[1];
            chatPolls.push({ hash: chat.hash, id: chat.id, isOpen: (this.OpenChat != null && chat.id == this.OpenChat.id), lastFetchedMessageId: chat.lastReadMessage.id })
        }
        $.ajax({
            url: `${this.RelativeUrl}Chat/PollForUpdates`,
            method: "POST",
            data: {
                chatsToPoll: chatPolls
            },
            success: (chats) => {
                this.ProcessChatUpdateDTOs(chats);
            }
        });
    }

    ProcessChatUpdateDTOs(chats) {
        const self = this;
        for (let chat of chats) {
            if (chat.isNewChat) {
                this.Chats.set(chat.newChat.id, chat.newChat);
                this.ChatListContainer.prepend(this.RenderChat(chat.newChat));
                $(`[ChatId="${chat.newChat.id}"]`).click((s) => {
                    self.FetchChatContent(
                        $(s.currentTarget).attr("ChatId")
                    );
                });
                return;
            }
            if (this.OpenChat != null && chat.id == this.OpenChat.id) {
                for (let message of chat.messages) {
                    let isYou = false;
                    if (message.user != null) {
                        isYou = message.user.id == this.LoggedInUserId;
                    }
                    $("#ChatContent").prepend(this.RenderMessage(message, isYou));
                }
                const chatItem = $(`[ChatId=${chat.id}]`);
                if (chat.messages.length > 0) {
                    this.Chats.get(chat.id).lastReadMessage = chat.messages[0];
                    $(".LastChatMessage", chatItem).text(chat.messages[chat.messages.length - 1].content);
                    document.getElementById('ChatContent').scrollTop = 0;
                }

                this.ChatListContainer.prepend(chatItem);

            } else if (chat.messages != null && chat.messages.length > 0) {
                if (chat.amountOfUnreadMessages != 0) {
                    const chatItem = $(`[ChatId=${chat.id}]`);
                    $(".GreenDot", chatItem).text(chat.amountOfUnreadMessages);
                    $(".GreenDot", chatItem).show();
                    this.ChatListContainer.prepend(chatItem);
                }
                $(".LastChatMessage", chatItem).text(chat.messages[0].content);
                this.Chats.get(chat.id).lastReadMessage = chat.messages[0];
            } else if (chat.removed == true) {
                $(`[ChatId=${chat.id}]`).remove();
            }
        }
    }

    RenderUser(user) {
        const self = this;
        const adminDisplay = user.isAdmin ? "block" : "none";
        let userElement = $(`<div class="UserListUser" ShowTargetPopupId="UserOptionsPopup"
                            PopupX="left" PopupY="bottom" />
                            <div class="UserListInfo">
                                <div class="UserListName">${user.firstName} ${user.lastName}</div>
                                <div style="display:${adminDisplay};" class="UserListAdmin">Admin</div>
                            </div>
                            <div class="UserListButtons">
                                <span class="material-symbols-outlined">more_vert</span>
                            </div>
                        </div >`);
        $("#UserList").append(userElement);
        if (this.OpenChat.isAdmin) {
            userElement.click(() => {
                self.SelectedUser.user = user;
                self.SelectedUser.element = userElement;
                ShowPopup.call(userElement[0])
            });
        }
    }

    RenderChat(chat) {
        if (chat.amountOfUnreadMessages > 0) {
            return ` <div ChatId="${chat.id}" class="ChatOuterContainer">
            <div class="ChatInnerContainer">
                <div class="ChatNameContainer">
                    <div class="ChatName">${chat.name}</div>
                    <div class="LastChatMessage">${chat.lastReadMessage.content}</div>
                </div>
                <div class="GreenDot">${chat.amountOfUnreadMessages}</div>
            </div>
        </div>`
        } else {
            return ` <div ChatId="${chat.id}" class="ChatOuterContainer">
            <div class="ChatInnerContainer">
                <div class="ChatNameContainer">
                    <div class="ChatName">${chat.name}</div>
                    <div class="LastChatMessage">${chat.lastReadMessage.content}</div>
                </div>
                 <div style="display:none;" class="GreenDot"></div>
            </div>
        </div>` 
        }
    }

    LoadChatContent(chat) {
        $("#ChatHeaderBar").text(chat.name);
        $("#ChatContent").empty();
        for (let message of chat.messages) {
            let isYou = false;
            if (message.user != null) {
                isYou = message.user.id == this.LoggedInUserId;
            }
            $("#ChatContent").append(this.RenderMessage(message, isYou));
        }

        this.OpenChat.isAdmin = this.OpenChat.users.some(user => user.id == this.LoggedInUserId && user.isAdmin);

        $("#UserList").empty();
        for (let user of chat.users) {
            this.RenderUser(user);
        }

        let chatItem = $(`[ChatId=${chat.id}]`);
        $(".GreenDot", chatItem).hide();
    }

    RenderSystemMessage(message) {
        return `<div class="SystemChatMessage">
                    <span class="MessageContent">
                        ${message.content}
                    </span>
                </div>`;
    }

    RenderMessage(message, isYou) {
        let cssClass = (isYou) ? "SenderChatMessage" : "ReceiverChatMessage";
        if (message.user == null) {
            return this.RenderSystemMessage(message);
        }
        return `<div class="${cssClass}">
                    <div class="MessageSender">
                        ${message.user.firstName} ${message.user.lastName}
                    </div>
                    <span class="MessageContent">
                        ${message.content}
                    </span>
                </div>`;
    }
}