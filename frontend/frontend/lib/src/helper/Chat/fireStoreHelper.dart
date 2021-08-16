import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:frontend/src/helper/UserInformation/userHelper.dart';
import 'package:frontend/src/models/Chat/ChatFirestoreUserModel.dart';
import 'package:frontend/src/models/Chat/ChatMessageModel.dart';
import 'package:rxdart/streams.dart';

class FireStoreHelper {
//TODO: get user id from db and create new message and user instance in firestore

  FirebaseFirestore firestore = FirebaseFirestore.instance;

  getUsersCollectionFromFireStore() {
    CollectionReference users = firestore.collection('users');

    print(users.doc().get());
  }

  createNewUsersDocsWithUid(uid, displayName, email, avatar) async {
    var refCollection = firestore.collection('users');
    var refChatsCollection = firestore.collection('chats').doc('$uid').set({});
    ChatFirestoreUserModel newUser = ChatFirestoreUserModel(
      uid: uid,
      displayName: displayName,
      email: email,
      avatar: avatar,
    );
    refCollection.add(newUser.toJson());
  }

//function adds user from inHouse api information to firestore users collection
  getUsersDocumentsFromFireStoreAsStream() {
    CollectionReference users = firestore.collection('users');

    return users.snapshots();
  }

  getChat(int idUser, int myId) {
    //get chats of user i want to talk to
    //filter those messages by searching for messages he sent to me
    return firestore
        .collection('chats/$myId/messages')
        .orderBy('dateCreated', descending: true)
        .snapshots();
  }

  Future sendMessage(int idUser, int myId, String message) async {
    //navigate to my messages
    final refMessagesMe = firestore.collection('chats/$myId/messages');
    final refMessagesThem = firestore.collection('chats/$idUser/messages');
    //create new message and show that you sent it
    final newMessage = ChatMessageModel(
      uid: myId,
      toUid: idUser,
      message: message,
      dateCreated: DateTime.now(),
    );

    //add new message to recipeint collection
    await refMessagesMe.add(
      newMessage.toJson(),
    );
    await refMessagesThem.add(
      newMessage.toJson(),
    );
    //update messaged user that he/she has a new message
    //final refUsers = firestore.collection('users/$idUser').doc().update({});
  }

  //function to get user object using user idUser
  Stream<QuerySnapshot<Map<String, dynamic>>> getUserById(idUser) {
    return firestore
        .collection('users')
        .where('uid', isEqualTo: idUser)
        .snapshots();
  }
}
