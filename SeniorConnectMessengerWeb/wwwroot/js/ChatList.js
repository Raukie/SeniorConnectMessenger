class ChatList {
    RelativeUrl;
    LoggedInUserId;
    constructor(loggedInUserId, relativeUrl = "/") {
        this.RelativeUrl = relativeUrl;
        this.FetchAllChats();
        this.LoggedInUserId = loggedInUserId;
        this.ChatContent = $("#ChatContent")
        this.Chats = new Map();
        this.ChatHeader;
    }

    Chats;
    ChatHeader;
    ChatContent;
    ChatListContainer = $("#ChatListInnerContainer");
    OpenChat;

    InitEvents() {
        const self = this;
        $(".ChatOuterContainer").click((s) => {
            self.FetchChatContent(
                $(s.currentTarget).attr("ChatId")
            );
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

    PollChats() {
        let chatPolls = [];
        for (let chatKeyValuePair of chatList.Chats) {
            let chat = chatKeyValuePair[1];
            chatPolls.push({ hash: chat.hash, id: chat.id, isOpen: (chat.id == this.OpenChat.id), lastFetchedMessageId: chat.lastReadMessage.id })
        }
        $.ajax({
            url: `${this.RelativeUrl}Chat/PollForUpdates`,
            method: "POST",
            data: {
                chatsToPoll: chatPolls
            },
            success: (chats) => {
                console.log(chats);
            }
        });
    }

    RenderChat(chat) {
        return ` <div ChatId="${chat.id}" class="ChatOuterContainer">
            <div class="ChatInnerContainer">
                <div class="ChatNameContainer">
                    <div class="ChatName">${chat.name}</div>
                    <div class="LastChatMessage">${chat.lastReadMessage.content}</div>
                </div>
                <div class="GreenDot">${chat.amountOfUnreadMessages}</div>
            </div>
        </div>` 
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
        debugger;
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