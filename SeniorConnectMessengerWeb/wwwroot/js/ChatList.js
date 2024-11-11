class ChatList {
    RelativeUrl;
    LoggedInUserId;
    constructor(loggedInUserId, relativeUrl = "/") {
        this.RelativeUrl = relativeUrl;
        this.RefreshAllChats();
        this.InitEvents();
        this.LoggedInUserId = loggedInUserId;
    }

    ChatHeader = $("#ChatHeader");
    ChatContent = $("#ChatContent");
    ChatListContainer = $("#ChatListInnerContainer");

    InitEvents() {
        const self = this;
    }

    RefreshAllChats() {
        $.ajax({
            url: `${this.RelativeUrl}Chat/GetUserChats`,
            method: "GET",
            success: (chats) => {
                this.ChatListContainer.empty();
                for (let chat of chats) {
                    this.ChatListContainer.append(this.RenderChat(chat));
                }
            }
        });
    }

    RenderChat(chat) {
        return ` <div class="ChatOuterContainer">
        <div class="ChatInnerContainer">
            <div class="ChatNameContainer">
                <div class="ChatName">${chat.name}</div>
                <div class="LastChatMessage">${chat.lastReadMessage.content}</div>
            </div>
            <div class="GreenDot">${chat.amountOfUnreadMessages}</div>
        </div>
    </div>` 
    }
}