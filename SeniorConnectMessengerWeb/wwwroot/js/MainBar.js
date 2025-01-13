class MainBar {
    RelativeUrl;
    constructor(relativeUrl = "/") {
        this.RelativeUrl = relativeUrl;
        this.InitEvents();
    }

    ChatButton = $("#ChatButton");
    MyPofileButton = $("#MyProfileButton");
    SettingsButton = $("#SettingsButton");

    ChatState = true;
    MyProfileState = false;
    SettingsState = false;

    InitEvents() {
        const self = this;
        this.ChatButton.click(() => { self.ClickedChat(true) });
        this.MyPofileButton.click(() => { self.ClickedMyProfile(true) });
        this.SettingsButton.click(() => { self.ClickedSettings(true) });
    }

    SetButtonActive(button, active) {
        debugger;
        if (active) {
            button.addClass("IconActive");
        } else {
            button.removeClass("IconActive");
        }
    }

    ClickedChat(state) {
        if (this.ChatState == state) return;
        this.ChatState = state;

        // deactivate other panels
        if (this.ChatState) {
            if (this.MyProfileState) this.ClickedMyProfile(false);
            if (this.SettingsState) this.ClickedSettings(false);
            $("#ChatListContainer").show();
        } else {
            $("#ChatListContainer").hide();
        }
        this.SetButtonActive(this.ChatButton, state);
    }

    ClickedMyProfile(state) {
        if (this.MyProfileState == state) return;
        this.MyProfileState = state;
        // deactivate other panels
        if (this.MyProfileState) {
            if (this.ChatState) this.ClickedChat(false);
            if (this.SettingsState) this.ClickedSettings(false);
            $("#ProfileInnerContainer").show();
        } else {
            $("#ProfileInnerContainer").hide();
        }

        this.SetButtonActive(this.MyPofileButton, state);
    }

    ClickedSettings(state) {
        if (this.SettingsState == state) return;
        this.SettingsState = state;
        // deactivate other panels
        if (this.SettingsState) {
            if (this.ChatState) this.ClickedChat(false);
            if (this.MyProfileState) this.ClickedMyProfile(false);
            $("#SettingsInnerContainer").show();
        } else {
            $("#SettingsInnerContainer").hide();
        }
        this.SetButtonActive(this.SettingsButton, state);
    }
}