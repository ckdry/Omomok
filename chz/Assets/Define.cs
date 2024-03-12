using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StreamerInfo
{
    public int code;
    public string message;
    public Content content;

    [Serializable]
    public class Content
    {
        public string channelId;
        public string channelName;
        public string channelImageUrl;
        public bool verifiedMark;
        public string channelType;
        public string channelDescription;
        public int followerCount;
        public bool openLive;
        public bool subscriptionAvailability;
        public SubscriptionPaymentAvailability subscriptionPaymentAvailability;
        [Serializable]
        public class SubscriptionPaymentAvailability
        {
            public bool iapAvailability;
            public bool iabAvailability;
        }
    }
}

[Serializable]
public class LiveStatus
{
    public int code;
    public string message;
    public Content content;

    [Serializable]
    public class Content
    {
        public string liveTitle;
        public string status;
        public int concurrentUserCount;
        public int accumulateCount;
        public bool paidPromotion;
        public bool adult;
        public string chatChannelId;
        public string categoryType;
        public string liveCategory;
        public string liveCategoryValue;
        public string livePollingStatusJson;
        public string faultStatus;
        public string userAdultStatus;
        public bool chatActive;
        public string chatAvailableGroup;
        public string chatAvailableCondition;
        public int minFollowerMinute;
    }
}

[Serializable]
public class AccessTokenResult
{
    public int code;
    public string message;
    public Content content;
    [Serializable]
    public class Content
    {
        public string accessToken;

        [Serializable]
        public class TemporaryRestrict
        {
            public bool temporaryRestrict;
            public int times;
            public int duration;
            public int createdTime;
        }
        public bool realNameAuth;
        public string extraToken;
    }
}

[Serializable]
public class Profile
{
    public string userIdHash;
    public string nickname;
    public string profileImageUrl;
    public string userRoleCode;
    public string badge;
    public string title;
    public string verifiedMark;
    public List<activityBadge> activityBadges;
    [Serializable]
    public class activityBadge
    {
        public int badgeNo;
        public string badgeId;
        public string imageUrl;
        public string title;
        public string description;
        public string activated;
    }
    public StreamingProperty streamingProperty;
    [Serializable]
    public class StreamingProperty
    {

    }
}


[Serializable]
public class DonationExtras
{
    public bool isAnonymous;
    public string payType;
    public int payAmount;
    public string nickname;
    public string donationType;

    public List<WeeklyRank> weeklyRankList;
    [Serializable]
    public class WeeklyRank
    {
        public string userIdHash;
        public string nickName;
        public bool verifiedMark;
        public int donationAmount;
        public int ranking;
        public long ctime;
        public long utime;
        public string msgTid;
        public long msgTime;
    }
    public int cmd;
    public string tid;
    public string cid;
}

[System.Serializable]
public class CellPosVote
{
    public string position;
    public int voteNum;
    public int row;
    public int col;
}